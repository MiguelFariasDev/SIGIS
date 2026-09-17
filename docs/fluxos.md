# SIGIS — Fluxos de negócio

> Fonte: `claude.md` (seções 1, 9 e 10). Este documento traduz os
> requisitos funcionais e as decisões-chave de design em fluxos passo a
> passo, para orientar a implementação dos casos de uso em
> `Sigis.Application/UseCases`. Fluxos marcados com **RFxx** referenciam
> o requisito correspondente em [requisitos.md](./requisitos.md).

## 1. Cadastro de pessoa com checagem de duplicidade (RF01, RF02, RF04)

```
Recepção/profissional preenche dados mínimos da pessoa
        |
        v
Sistema normaliza nome (unaccent + lower) e dispara busca de candidatos
a duplicidade ANTES de confirmar o cadastro:
        |
        +-- 1. Match exato por CNS (confiança máxima)
        |
        +-- 2. Match exato por CPF (quando CNS ausente — comum no NAPE)
        |
        +-- 3. Match probabilístico: pg_trgm sobre nome_normalizado
        |      + filtro duro por data_nascimento + limiar de similaridade >= 0.75
        v
Existem candidatos?
        |
   não  |  sim
        |   |
        v   v
   Confirma   Exibe candidatos com score de similaridade ao usuário
   cadastro   antes de confirmar
   normal          |
        |           +-- Usuário confirma "é pessoa diferente" -> segue com
        |           |    o cadastro normalmente (RF01)
        |           |
        |           +-- Usuário reconhece que pode ser a mesma pessoa ->
        |                cadastro é criado (ou cancelado) e um
        |                ALERTA_DUPLICIDADE fica pendente de revisão
        |                por um COORDENADOR (RF04) — ver seção 2
        v
   PESSOA criada em sigis.pessoa
```

**Regra crítica (não questionar):** o sistema **nunca** funde cadastros
automaticamente, mesmo com match exato por CNS. Toda duplicidade
detectada gera um `ALERTA_DUPLICIDADE` com `status = PENDENTE` para
revisão humana.

### SQL de referência (candidatos a duplicidade)

```sql
SELECT
  p2.id,
  similarity(p1.nome_normalizado, p2.nome_normalizado) AS score_nome
FROM sigis.pessoa p1
JOIN sigis.pessoa p2
  ON p1.id <> p2.id
 AND p1.data_nascimento = p2.data_nascimento
 AND p1.nome_normalizado % p2.nome_normalizado
WHERE similarity(p1.nome_normalizado, p2.nome_normalizado) >= 0.75
ORDER BY score_nome DESC
LIMIT 10;
```

## 2. Resolução de duplicidade / merge de cadastros (RF03, RF04)

```
COORDENADOR abre a fila de ALERTA_DUPLICIDADE (status = PENDENTE)
        |
        v
Para cada alerta, visualiza pessoa_id_1, pessoa_id_2 e score_similaridade
        |
        v
Decide:
        |
        +-- MESCLAR: os dois cadastros são a mesma pessoa
        |      |
        |      v
        |   Sistema mescla os cadastros preservando o histórico de
        |   ATENDIMENTO e ENCAMINHAMENTO de ambos sob um identificador
        |   canônico (RF03). ALERTA_DUPLICIDADE.status = MESCLADO,
        |   resolvido_por_profissional_id e resolvido_em são gravados.
        |
        +-- FALSO POSITIVO: são pessoas diferentes
               |
               v
            ALERTA_DUPLICIDADE.status = FALSO_POSITIVO, ambos os
            cadastros permanecem intactos e separados.
```

Este fluxo só pode ser executado por um profissional com `papel_rbac =
COORDENADOR` (escopo: rede completa) — ver [governanca-dados.md](./governanca-dados.md).

## 3. Entrada e priorização na fila de atendimento (RF05)

```
Pessoa é encaminhada para um serviço (ou procura diretamente a unidade)
        |
        v
Unidade abre um registro em FILA_ATENDIMENTO:
  pessoa_id, unidade_id, especialidade, prioridade, entrada_fila,
  status = AGUARDANDO
        |
        v
prioridade é uma das categorias já usadas pelo NAPE:
  URGENTE | CURTO_PRAZO | LISTA_ESPERA
        |
        v
Índice único parcial garante que a mesma pessoa não fique duas vezes
ativa na fila da mesma unidade:

  CREATE UNIQUE INDEX idx_fila_ativa_unica
    ON sigis.fila_atendimento (pessoa_id, unidade_id)
    WHERE status IN ('AGUARDANDO', 'EM_ATENDIMENTO');
        |
        v
Fila é visível e reordenável pela própria unidade (escopo RBAC:
PROFISSIONAL vê só a fila da própria unidade)
```

Transições de `status`: `AGUARDANDO -> EM_ATENDIMENTO -> CONCLUIDO`, ou
`AGUARDANDO -> FALTOU`, com possibilidade de `BUSCA_ATIVA` quando a
unidade decide reengajar um paciente faltoso.

## 4. Registro de atendimento e alerta de atendimento concorrente (RF06, RF09, RF10)

```
Profissional abre um atendimento agendado (item da fila ou consulta avulsa)
        |
        v
Sistema verifica: pessoa já tem atendimento ATIVO em outro serviço
da rede? (RF10 — distinto de RF02: aqui é duplicidade de ATENDIMENTO,
não de CADASTRO)
        |
   não  |  sim
        |   |
        v   v
   segue   Exibe alerta visual ao profissional (ex.: "paciente já em
   normal  atendimento ativo na Casa Mais Azul desde dd/mm") — decisão de
        |  prosseguir ou não é do profissional, sistema não bloqueia
        v
Profissional registra o atendimento:
  ATENDIMENTO { pessoa_id, unidade_id, profissional_id, data_hora,
                tipo_sessao, dados_formulario (jsonb), session_number }
        |
        v
tipo_sessao determina qual template de formulário é exibido no
frontend (estático para o hackathon):
  ANAMNESE_PSI | ANAMNESE_PSICOPED | INSTRUMENTAL_EDFISICA |
  PRONTUARIO_NASF | SINTESE
        |
        v
Ao final (ou no não comparecimento), grava comparecimento:
  AGENDADO -> COMPARECEU | FALTOU
```

**Por que `dados_formulario` é JSONB e não colunas fixas:** ver
[system-design.md](./system-design.md), seção "Por que `dados_formulario`
é JSONB". Cada serviço (NASF, NAPE — com suas 5 fichas distintas)
preserva sua própria estrutura de formulário sem exigir migração de
schema a cada novo campo.

## 5. Encaminhamento entre unidades (RF07, RF08)

```
Profissional decide encaminhar pessoa para outra unidade da rede
        |
        v
ENCAMINHAMENTO criado:
  pessoa_id, unidade_origem_id, unidade_destino_id, motivo, prioridade,
  data_encaminhamento, status = PENDENTE
        |
        v
Unidade destino avalia:
        |
        +-- ACEITO -> encaminhamento entra no fluxo de fila da unidade
        |             destino (ver seção 3)
        |
        +-- RECUSADO -> encaminhamento fica registrado como recusado,
        |               unidade origem é notificada
        |
        +-- (após atendimento realizado) -> CONCLUIDO
        v
Todo ATENDIMENTO e ENCAMINHAMENTO da pessoa, em qualquer unidade,
aparece na timeline consolidada (RF08) — sujeita a controle de acesso
cross-unidade (ver seção 6 e governanca-dados.md).
```

## 6. Acesso cross-unidade e timeline consolidada (RF08, RF12, RF13)

```
Profissional de uma unidade tenta abrir a timeline consolidada
(RF08) de uma pessoa atendida também em OUTRAS unidades
        |
        v
Por padrão, RBAC restringe visão a dados da própria unidade (RF12)
        |
        v
Para ver a timeline cross-unidade, o sistema exige
justificativa/consentimento registrado (RF12)
        |
        v
Acesso concedido -> LOG_ACESSO é gravado obrigatoriamente:
  pessoa_id, profissional_id, acao, base_legal, data_hora (RF13, RNF03)
        |
        v
AUDITOR/DPO pode consultar LOG_ACESSO (somente leitura) a qualquer
momento — ver governanca-dados.md e lgpd.md
```

## 7. Painel de indicadores (RF11)

O painel agrega, por período e por unidade/rede:

- nº de atendimentos
- nº de faltas (`comparecimento = FALTOU`)
- nº de encaminhamentos ativos (`status IN (PENDENTE, ACEITO)`)
- nº de duplicidades pendentes (`ALERTA_DUPLICIDADE.status = PENDENTE`)

Como `dados_formulario` é JSONB, indicadores que dependam de campos
específicos de um serviço usam os operadores nativos do PostgreSQL
(`->>`, `@>`) sobre essa coluna — ver
[system-design.md](./system-design.md).

## 8. Uso mobile offline-first (RF14)

```
Recepção/profissional em campo usa o app Flutter sem conectividade
        |
        v
connectivity_plus detecta ausência de rede
        |
        v
Ações permitidas em modo degradado (RNF06):
  - Buscar paciente no cache local (sqflite) — minimizado por LGPD,
    não cacheia histórico completo (ver lgpd.md)
  - Confirmar comparecimento/falta
  - Registrar sessão rápida
        |
        v
Ações ficam em fila local (sqflite)
        |
        v
Conectividade retorna -> fila local sincroniza com a API
```

## 9. O que ainda falta detalhar

Regras de negócio críticas adicionais e o roadmap detalhado do hackathon
existem como títulos no índice do `claude.md` (seções 14 e 17) mas seu
conteúdo ainda não foi preenchido no arquivo fonte — os fluxos acima
cobrem o que está documentado nas seções 1, 9 e 10. Atualizar este
documento assim que o conteúdo completo estiver disponível em
[SIGIS-MESTRE.md](./SIGIS-MESTRE.md).
