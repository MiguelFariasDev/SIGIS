# SIGIS — Governança de dados

> Fonte: `claude.md` (seções 5.5, 7, 9.1–9.5). Este documento descreve
> quem pode acessar o quê, como a identidade de uma pessoa é mantida
> única na rede, e como a qualidade/integridade dos dados é preservada ao
> longo do ciclo de vida do cadastro.

## 1. Identidade canônica vs. registro de atendimento

A decisão de governança mais importante do projeto: **existe uma única
fonte de verdade de identidade** (`PESSOA`), separada dos registros de
atendimento específicos de cada serviço (`ATENDIMENTO`).

- **Por quê:** NASF e NAPE têm fichas completamente diferentes, com
  campos que quase não se sobrepõem. Um "prontuário único de campos
  fixos" geraria uma tabela frágil, cheia de colunas nulas, e quebraria
  a cada novo tipo de anamnese.
- **Como:** `PESSOA` concentra os dados de identidade compartilhados por
  toda a rede (nome, nascimento, CNS, CPF, responsável). `ATENDIMENTO`
  referencia `PESSOA` via `pessoa_id` e guarda em `dados_formulario`
  (JSONB) os campos específicos daquele serviço/sessão.

Isso é o que permite ao sistema resolver as dores 2 e 4 do diagnóstico da
Secretaria de Saúde (duplicidade de atendimento e fragmentação de
informação — ver [requisitos.md](./requisitos.md)) sem forçar os
profissionais a preencher um formulário genérico que não corresponde à
sua prática real.

## 2. Ciclo de vida da identidade (deduplicação)

A unicidade de `PESSOA` na base é mantida por um processo de
deduplicação em 3 camadas, **nunca automático**:

| Camada | Método | Confiança |
|---|---|---|
| 1 | Match exato por CNS | Máxima — mas merge ainda passa por revisão humana |
| 2 | Match exato por CPF | Usado quando CNS ausente (comum no NAPE) |
| 3 | Match probabilístico | `pg_trgm` sobre nome normalizado (`unaccent+lower`) + filtro duro por data de nascimento + limiar de similaridade 0.75 |

Toda ocorrência de match gera um registro em `ALERTA_DUPLICIDADE`
(`pessoa_id_1`, `pessoa_id_2`, `score_similaridade`, `status`). Um
`COORDENADOR` decide entre `MESCLADO` (funde os cadastros preservando o
histórico de ambos sob um identificador canônico — RF03) ou
`FALSO_POSITIVO` (mantém os dois cadastros separados). Ver o fluxo
completo em [fluxos.md](./fluxos.md), seções 1 e 2.

**Nunca fundir cadastros automaticamente** é uma regra crítica: uma
fusão incorreta mistura o histórico clínico/pedagógico de duas pessoas
diferentes, com consequências diretas para o cuidado e para a LGPD (ver
[lgpd.md](./lgpd.md)).

## 3. Controle de acesso — RBAC de 3 papéis

| Papel | Escopo de dados | Pode... |
|---|---|---|
| `PROFISSIONAL` | Própria unidade | Consultar histórico da unidade, gerenciar fila, registrar atendimento |
| `COORDENADOR` | Rede completa | Tudo que `PROFISSIONAL` faz + resolver `ALERTA_DUPLICIDADE` (merge/falso positivo) + ver indicadores da rede (RF11) |
| `AUDITOR` | Somente leitura de `LOG_ACESSO` | Consultar logs de acesso e compartilhamento — sem acesso aos dados clínicos |

Regras derivadas (RF12):

- Por padrão, todo `PROFISSIONAL` vê apenas dados da própria unidade.
- Acesso à timeline consolidada entre unidades (RF08) exige
  justificativa/consentimento registrado — não é um acesso "de graça"
  mesmo para quem está autenticado.
- Todo acesso cross-unidade gera entrada em `LOG_ACESSO` (RF13),
  consultável pelo `AUDITOR` — ver [lgpd.md](./lgpd.md).

## 4. Módulos de domínio e responsabilidade sobre os dados

Refletindo a estrutura de `Sigis.Application/UseCases` (ver
[system-design.md](./system-design.md)):

- **Identidade e Autorização** — dono de usuários, papéis (RBAC), JWT.
- **Cadastro Único** — dono de `PESSOA`, `RESPONSAVEL`,
  `UNIDADE_SERVICO`, `PROFISSIONAL`.
- **Fila de Atendimento** — dono de `FILA_ATENDIMENTO` e das regras de
  priorização/unicidade (índice único parcial por pessoa+unidade ativa).
- **Atendimentos e Histórico** — dono de `ATENDIMENTO`, `ENCAMINHAMENTO`
  e da timeline consolidada.
- **Deduplicação** — dono de `ALERTA_DUPLICIDADE` e do processo de
  matching (seção 2 acima).

Cada módulo é o único responsável por escrever nas tabelas que possui;
leitura cross-módulo acontece via casos de uso explícitos (MediatR), não
via acesso direto a `DbContext` fora de `Sigis.Infrastructure` — ver as
regras de "Não fazer" em [system-design.md](./system-design.md).

## 5. Qualidade e integridade dos dados

- **Normalização de nome:** `nome_normalizado` é uma coluna **gerada**
  (`GENERATED ALWAYS AS (unaccent(lower(nome_completo))) STORED`), não
  mantida manualmente pela aplicação — evita divergência entre nome
  original e nome usado para matching.
- **Unicidade de fila ativa:** índice único parcial impede que a mesma
  pessoa tenha duas entradas simultaneamente ativas
  (`AGUARDANDO`/`EM_ATENDIMENTO`) na fila da mesma unidade.
- **Dados semiestruturados controlados:** `dados_formulario` é JSONB,
  mas `tipo_sessao` (enum) ancora qual template/formato é esperado
  naquele documento — a flexibilidade do JSONB não significa ausência de
  contrato, apenas contrato definido no nível da aplicação/frontend em
  vez do nível do schema relacional.

## 6. Indicadores de governança (RF11)

O painel de indicadores (ver [fluxos.md](./fluxos.md), seção 7) serve
também como sinal de saúde da governança de dados:

- nº de duplicidades pendentes — mede o atraso na revisão de
  `ALERTA_DUPLICIDADE`; um número crescente indica risco de dados
  fragmentados na rede.
- nº de encaminhamentos ativos — mede continuidade do cuidado entre
  unidades.
- nº de faltas — indicador operacional, mas também sinaliza pacientes
  que podem precisar de busca ativa.

## 7. O que ainda falta detalhar

Regras de negócio críticas adicionais (seção 14 do `claude.md`) e
anti-requisitos (seção 15) constam no índice do `claude.md` mas seu
conteúdo não estava presente no arquivo fonte no momento da geração
deste documento. Atualizar esta página quando esse conteúdo estiver
disponível em [SIGIS-MESTRE.md](./SIGIS-MESTRE.md).
