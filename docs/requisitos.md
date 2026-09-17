# SIGIS — Requisitos

> Fonte: `claude.md` (seções 1, 2, 10 e 11). Este documento consolida o
> problema de negócio, as personas e os requisitos funcionais/não-funcionais
> em um único lugar para consulta rápida durante o desenvolvimento e para
> apresentação à banca.

## 1. Contexto

O SIGIS — Sistema Integrado de Gestão e Informação em Saúde — é um sistema
para a Secretaria Municipal de Saúde de Crateús/CE gerenciar o cuidado a
pessoas com Transtorno do Espectro Autista (TEA) na rede pública de saúde.

- **Demandante:** Secretaria Municipal de Saúde de Crateús
- **Proponente técnico:** SEPLATI — Secretaria Municipal de Planejamento e
  Tecnologia da Informação
- **Contexto:** projeto de hackathon (13/09/2026)
- **Documento mestre:** [SIGIS-MESTRE.md](./SIGIS-MESTRE.md)

### 1.1 As 4 dores que o sistema resolve

O diagnóstico da Secretaria Municipal de Saúde aponta quatro dores concretas,
mantidas como norte durante todo o desenvolvimento — a banca avalia
aderência ao problema real:

1. Inexistência de sistema informatizado para gerir inserção e
   acompanhamento de pacientes com TEA na rede pública.
2. Ausência de mecanismo para detectar duplicidade de atendimento entre
   NASF, CREAES, NAPE, Casa Mais Azul e CRASF.
3. Controle de filas e histórico em cadernos físicos.
4. Fragmentação de informações entre serviços, comprometendo a
   continuidade do cuidado.

### 1.2 Leitura estratégica do diagnóstico

NASF e NAPE têm fichas completamente diferentes:

- **NASF** registra prontuário clínico simples: número, hipótese
  diagnóstica, CNS, filiação, dados do responsável.
- **NAPE** registra anamneses extensas por profissional: psicológica,
  psicopedagógica, instrumental de educação física, síntese de
  acompanhamento — **sem** prontuário único nem entre os próprios
  profissionais do NAPE.

Os campos quase não se sobrepõem. Isso significa que a solução **não pode**
assumir "prontuário único de campos fixos". A arquitetura precisa separar:

- **(a)** Núcleo de identidade da pessoa (comum a todos os serviços) —
  resolve duplicidade e fragmentação.
- **(b)** Registros de atendimento específicos por serviço (preservando a
  natureza de cada anamnese) — sem forçar formulário genérico que nenhum
  profissional preencheria de verdade.

Essa distinção é a decisão de design mais importante do projeto (ver
[system-design.md](./system-design.md), seção "Decisões-chave de design").

### 1.3 Serviços da rede municipal envolvidos

| Sigla | Nome | Secretaria |
|---|---|---|
| NASF | Núcleo Ampliado de Saúde da Família | Saúde |
| CREAES | — | Saúde |
| NAPE | Núcleo de Atendimento Pedagógico Especializado | Educação |
| Casa Mais Azul | — | Saúde |
| CRASF | — | Assistência Social |

## 2. Personas e casos de uso

| Persona | Onde atua | Necessidade principal |
|---|---|---|
| Coordenador(a) Secretaria de Saúde/SEPLATI | Painel web | Visão consolidada da rede, indicadores, resolver duplicidades |
| Profissional de atendimento (NASF, NAPE, CREAES, Casa Mais Azul, CRASF) | Painel web e/ou app mobile | Consultar histórico, gerenciar fila da própria unidade, registrar atendimento |
| Recepção/triagem da unidade | App mobile (balcão/campo) | Buscar paciente, confirmar comparecimento/falta, abrir cadastro com checagem de duplicidade |
| Auditor/DPO municipal | Painel web restrito | Consultar logs de acesso e compartilhamento |

## 3. Requisitos Funcionais (RF)

### 3.1 Núcleo de cadastro e identidade

- **RF01** — Cadastrar pessoa com TEA (ou em investigação) com dados
  mínimos: nome, data de nascimento, CNS, CPF (opcional), nome da mãe,
  sexo, endereço, telefone, dados do responsável.
- **RF02** — Ao cadastrar, o sistema deve buscar automaticamente possíveis
  duplicatas **antes** de confirmar o cadastro.
- **RF03** — Permitir mesclar (merge) dois cadastros identificados como a
  mesma pessoa, preservando o histórico de ambos sob um identificador
  canônico.
- **RF04** — Manter fila de duplicidades pendentes de revisão manual, com
  indicador de confiança do match.

### 3.2 Fila e atendimento

- **RF05** — Cada unidade gerencia sua própria fila, visível e
  priorizável (urgente / curto prazo / lista de espera — categorias já
  usadas pelo NAPE).
- **RF06** — Registrar comparecimento ou falta em cada atendimento
  agendado/realizado.
- **RF07** — Registrar encaminhamento de paciente entre unidades, com
  motivo e prioridade.
- **RF08** — Exibir, por paciente, linha do tempo consolidada de
  atendimentos e encaminhamentos entre **todos** os serviços da rede.

### 3.3 Registro clínico/pedagógico específico por serviço

- **RF09** — Cada serviço registra seu próprio tipo de sessão/anamnese,
  com campos próprios. Estrutura mínima: templates de formulário por
  serviço, armazenados como dados semiestruturados (JSONB).
- **RF10** — Alertar visualmente quando paciente já possui atendimento
  ativo em outro serviço da rede, no momento em que novo atendimento é
  aberto. *(Distinto do RF02: RF02 = duplicidade de **cadastro**; RF10 =
  duplicidade de **atendimento**.)*

### 3.4 Painel e indicadores

- **RF11** — Painel com: nº de atendimentos por período, nº de faltas,
  nº de encaminhamentos ativos, nº de duplicidades pendentes.

### 3.5 Segurança e compartilhamento

- **RF12** — RBAC: cada profissional vê, por padrão, apenas dados da
  própria unidade. Linha do tempo consolidada (RF08) exige
  justificativa/consentimento registrado para acesso cross-unidade.
- **RF13** — Registrar log de auditoria de todo acesso a dados de
  paciente por profissional de unidade diferente da originadora.

### 3.6 Mobile

- **RF14** — App mobile permite busca de paciente, confirmação de
  comparecimento/falta e registro de sessão rápida — offline-first.

## 4. Requisitos Não-Funcionais (RNF)

| # | Requisito | Como é atendido |
|---|---|---|
| RNF01 | Interoperabilidade sem acoplamento rígido: cada serviço pode evoluir seu formulário sem migração de schema | `Atendimento.dados_formulario` como JSONB — ver [system-design.md](./system-design.md) |
| RNF02 | LGPD (Lei 13.709/2018): dado de saúde é sensível (art. 5º, II). Base legal explícita (art. 11, II, "a" e "f"). Campo `base_legal` gravado, não checkbox decorativo | ver [lgpd.md](./lgpd.md) |
| RNF03 | Auditabilidade: nenhum dado sensível é lido cross-unidade sem deixar rastro (RF13) | `LOG_ACESSO` — ver [governanca-dados.md](./governanca-dados.md) |
| RNF04 | Usabilidade para não-especialistas em TI: agentes de saúde e educação, não desenvolvedores, operam o sistema. Minimizar campos obrigatórios e usar linguagem do domínio | orienta design de telas (fora do escopo deste documento) |
| RNF05 | Baixo custo: monolito modular (não microsserviços) — menos infraestrutura para manter com verba pública municipal | ver [system-design.md](./system-design.md), "Monolito modular" |
| RNF06 | Resiliência de conectividade no mobile: modo degradado com fila local + sincronização | `sqflite` + fila offline no Flutter |
| RNF07 | Performance de busca: < 300 ms mesmo com matching fuzzy | índice GIN `pg_trgm` sobre `nome_normalizado` |

## 5. O que ainda falta detalhar

As seguintes seções existem no índice do `claude.md` mas seu conteúdo ainda
não foi preenchido no arquivo fonte no momento da geração deste documento —
atualizar aqui assim que estiverem disponíveis:

- Endpoints da API (seção 12)
- Comandos úteis (seção 13)
- Regras de negócio críticas (seção 14)
- Anti-requisitos (seção 15)
- LGPD — pontos obrigatórios, versão detalhada (seção 16)
- Roadmap do hackathon (seção 17)
- Glossário (seção 20)

Ver também o documento mestre completo em
[SIGIS-MESTRE.md](./SIGIS-MESTRE.md) quando disponível.
