# SIGIS — System Design

> Fonte: `claude.md` (seções 3, 4, 5, 6, 7, 8 e 9). Este documento descreve
> a stack, a arquitetura, as convenções de código e o modelo de dados do
> SIGIS. Decisões marcadas como "não questionar" no `claude.md` estão
> reproduzidas aqui com sua justificativa — não devem ser revertidas sem
> motivo forte.

## 1. Stack obrigatória

### Backend
- .NET 10 (`net10.0`) + ASP.NET Core com Minimal APIs
- Clean Architecture
- MediatR 12.x para CQRS leve
- FluentValidation 11.x para validação
- AutoMapper 14.x para mapeamento
- EF Core 10 + Npgsql.EntityFrameworkCore.PostgreSQL 10.x
- Hangfire para jobs em background (**opcional** no MVP)
- Serilog para logging estruturado
- Swashbuckle para Swagger
- Asp.Versioning.Mvc para versionamento

**Sem API Gateway** (YARP, Nginx, Ocelot) — a API fala diretamente com os
clientes (React e Flutter). Ver seção 4.2 abaixo.

### Banco de dados
- PostgreSQL 16 (`postgres:16-alpine`)
- Extensões obrigatórias: `pg_trgm`, `unaccent`, `pgcrypto`
- Schema: `sigis` · Banco: `sigis_db` · Usuário: `sigis` · Senha (dev): `sigis_dev`

### Frontend
- React 19 + Vite 6 + TypeScript 5.7+
- TanStack Query v5 para data fetching
- Tailwind CSS 4 (via plugin Vite — sem `tailwind.config.js`)
- shadcn/ui (canary compatível com React 19)
- React Router v7
- Axios para HTTP · Zod para validação de formulários

### Mobile
- Flutter 3.27+ (Dart 3.6+)
- Riverpod 2 para state management
- sqflite ou Hive para persistência local (offline-first)
- Dio para HTTP
- connectivity_plus para detecção de rede
- shared_preferences para configurações

### Infraestrutura
Apenas 3 containers obrigatórios: `sigis-db` (PostgreSQL 16), `sigis-api`
(.NET 10), `sigis-front` (React 19 + Vite dev).


**Não incluir:** API Gateway, Nginx, Traefik, Keycloak, Kong, RabbitMQ,
Kafka, Kubernetes, Prometheus, Grafana, Seq.

### CI
GitHub Actions com 3 jobs: backend (.NET 10), frontend (Node 22), mobile
(Flutter stable). Roda em push e pull_request para `main` e `develop`.

## 2. Estrutura do repositório

```
sigis/
├── claude.md
├── docker-compose.yml
├── Makefile
├── docs/
│   ├── SIGIS-MESTRE.md
│   ├── requisitos.md
│   ├── system-design.md   <- este arquivo
│   ├── fluxos.md
│   ├── lgpd.md
│   └── governanca-dados.md
├── sigis-backend/
│   ├── Sigis.sln
│   ├── src/
│   │   ├── Sigis.Domain/
│   │   ├── Sigis.Application/
│   │   ├── Sigis.Infrastructure/
│   │   ├── Sigis.Api/
│   │   └── Sigis.Mobile.Contracts/
│   └── tests/
│       ├── Sigis.Domain.Tests/
│       └── Sigis.Application.Tests/
├── sigis-frontend/
│   ├── src/
│   │   ├── features/
│   │   ├── components/
│   │   ├── lib/api/
│   │   ├── routes/
│   │   └── templates/
│   └── public/
└── sigis-mobile/
    └── lib/
        ├── features/
        ├── data/
        └── core/
```

## 3. Arquitetura — Clean Architecture

### 3.1 Visão de alto nível (sem gateway)

```
+--------------------------------------------------------------------------+
|                            CLIENTES                                      |
|  +---------------------+  +---------------------+  +-----------------+  |
|  |  React SPA          |  |  App Flutter        |  |  Auditor / DPO  |  |
|  |  Painel da Rede     |  |  Uso em Campo       |  |  (web restrito) |  |
|  |  (gestor/prof)      |  |  (check-in, busca)  |  |                 |  |
|  +----------+----------+  +----------+----------+  +--------+--------+  |
+-------------+-------------------------+---------------------+-----------+
              | HTTPS REST/JSON         | HTTPS REST/JSON     |
              | (direto, sem gateway)   | + fila offline      |
              v                         v                     v
+--------------------------------------------------------------------------+
|                  Sigis.Api - ASP.NET Core 10 (Monolito Modular)          |
|                  http://localhost:5000                                   |
|                                                                          |
|  +--------------+ +--------------+ +--------------+ +----------------+  |
|  |  Módulo      | |  Módulo      | |  Módulo      | |  Módulo        |  |
|  |  Identidade  | |  Cadastro    | |  Fila de     | |  Atendimentos  |  |
|  |  e Auth      | |  Único       | |  Atendimento | |  e Histórico   |  |
|  +--------------+ +--------------+ +--------------+ +----------------+  |
|  +--------------+ +--------------------------------------------------+  |
|  |  Módulo      | |  MediatR . FluentValidation . Pipeline Behaviors |  |
|  |  Deduplicação| |  (validação, logging, auditoria)                 |  |
|  +--------------+ +--------------------------------------------------+  |
|                                                                          |
|  Middlewares embutidos: CORS, Exception Handler, Request Logging, JWT   |
+------------------------------+-------------------------------------------+
                               v
       +-----------------------+------------------------+
       v                       v                        v
+--------------+       +--------------+         +--------------+
| PostgreSQL16 |       |  Redis       |         |  MinIO / S3  |
| + pg_trgm    |       |  (opcional)  |         |  (opcional)  |
| + unaccent   |       |              |         |              |
| + pgcrypto   |       |              |         |              |
+--------------+       +--------------+         +--------------+
```

### 3.2 Por que sem API Gateway *(decisão de hackathon — não questionar)*

- Gateway (YARP, Nginx, Ocelot) resolve problema de múltiplos serviços
  atrás de um ponto único. O backend é **um** monolito modular — não há
  nada para rotear.
- Cada camada a mais consome tempo de configuração, debug de CORS duplo,
  certificados, healthchecks e rede Docker.
- CORS, autenticação e rate limit ficam dentro da própria API
  (middlewares nativos do ASP.NET Core), padrão recomendado para
  monolitos.
- Em produção futura, se a Prefeitura quiser expor a API publicamente, a
  adição de um Nginx na frente é trivial e não exige mudança de código.

**Regra prática:** nenhuma menção a gateway, proxy reverso ou service
mesh neste projeto durante o hackathon.

### 3.3 Por que monolito modular, não microsserviços

Dado RNF05 (baixo custo) e prazo de hackathon, a escolha correta é
monolito modular em Clean Architecture — em vez de microsserviços, que
resolveriam um problema de escala que este sistema não tem (prefeitura de
porte médio, não rede nacional). O custo de coordenação (múltiplos
deploys, comunicação assíncrona, observabilidade distribuída) é melhor
investido em requisitos funcionais nas 20h de hackathon.

A modularidade fica **dentro** do processo: módulos de domínio bem
isolados (Cadastro, Fila, Atendimento, Deduplicação), comunicando-se via
MediatR e eventos de domínio in-process. Preserva a possibilidade de
extrair um módulo como serviço separado no futuro, sem pagar o custo
agora.

### 3.4 Camadas e dependências

```
Sigis.Api              (apresentação — Controllers/Minimal APIs)
    |  depende de
    v
Sigis.Infrastructure   (implementação — EF Core, repositórios, pg_trgm)
    |  depende de
    v
Sigis.Application      (orquestração — CQRS, interfaces, validators)
    |  depende de
    v
Sigis.Domain           (núcleo — entidades, VOs, regras de negócio puras)
```

**Regra de ouro:** dependências fluem de fora para dentro. Domain não
conhece ninguém. Application conhece Domain. Infrastructure conhece
Application e Domain. Api conhece Application e Infrastructure.

### 3.5 Módulos de domínio (dentro de cada camada)

- **Identidade e Autorização** — usuários, papéis, JWT
- **Cadastro Único** — Pessoa, Responsável, UnidadeServico, Profissional
- **Fila de Atendimento** — FilaAtendimento, priorização
- **Atendimentos e Histórico** — Atendimento, Encaminhamento, timeline
- **Deduplicação** — AlertaDuplicidade, matching pg_trgm

### 3.6 Comunicação entre clientes e API

- Frontend React chama a API em `http://localhost:5000` (dev) ou
  `VITE_API_URL` configurada no `.env`.
- Mobile Flutter chama a API em `http://10.0.2.2:5000` (emulador
  Android), `http://localhost:5000` (iOS/desktop) ou IP da máquina na
  rede local.
- CORS configurado no `Program.cs` da API para aceitar
  `http://localhost:5173` e `http://localhost:3000` (e origens
  Capacitor/Cordova se necessário).
- Autenticação via JWT Bearer no header `Authorization`.
- Sem proxy reverso, sem gateway, sem Nginx.

### 3.7 O que não incluir na arquitetura (durante o hackathon)

- API Gateway (YARP, Ocelot, Kong, Traefik)
- Nginx como reverse proxy
- Service mesh (Istio, Linkerd)
- Keycloak ou IdentityServer externo (usar JWT nativo)
- Message broker externo (RabbitMQ, Kafka) — usar MediatR in-process
- Kubernetes — usar Docker Compose
- Elastic Stack / Grafana / Prometheus — usar Serilog (+ Seq opcional)

## 4. Convenções de código

### 4.1 Nomes e idioma

Idioma do código: **português** (alinhado ao domínio).

- Classes: `Pessoa`, `Atendimento`, `FilaAtendimento`
- Propriedades: `NomeCompleto`, `DataNascimento`, `CriadoEm`
- Métodos: `RegistrarAtendimento()`, `MesclarCadastros()`
- Tabelas: `pessoa`, `atendimento`, `fila_atendimento` (snake_case)
- Colunas: `nome_completo`, `data_nascimento` (snake_case)

Exceções: tipos técnicos e palavras reservadas ficam em inglês —
`IRepository<T>`, `DbContext`, `ILogger<T>`, `Task<T>`, `async`/`await`.

### 4.2 Estrutura de pastas por projeto

```
Sigis.Domain/
├── Entities/           (Pessoa, Atendimento, ...)
├── ValueObjects/       (Cns, Cpf, NomeCompleto, ...)
├── Enums/              (PrioridadeFila, StatusFila, ...)
├── Events/             (eventos de domínio)
├── Exceptions/         (DomainException, ...)
└── Interfaces/         (interfaces puras do domínio)

Sigis.Application/
├── UseCases/           (Pessoas/ Filas/ Atendimentos/ Encaminhamentos/ Duplicidades/)
├── Interfaces/         (interfaces de repositórios e serviços)
├── Validators/         (FluentValidation)
├── DTOs/
├── Mappings/           (AutoMapper profiles)
└── Behaviors/          (pipeline behaviors do MediatR)

Sigis.Infrastructure/
├── Persistence/
│   ├── Context/        (SigisDbContext)
│   ├── Configurations/ (IEntityTypeConfiguration<T>)
│   ├── Migrations/
│   └── Repositories/
├── Matching/            (deduplicação com pg_trgm)
├── Jobs/                (opcional no MVP)
├── Storage/             (MinIO client — opcional no MVP)
└── Extensions/

Sigis.Api/
├── Endpoints/           (Minimal APIs por módulo)
├── Middlewares/
├── Extensions/
├── Filters/
├── Contracts/           (request/response da API)
└── Program.cs
```

### 4.3 Padrões obrigatórios

- **Result Pattern:** casos de uso retornam `Result<T>`/`Result` (nunca
  lançam exceção para fluxo esperado).
- **CQRS leve com MediatR:** cada caso de uso é um `IRequest<T>` com seu
  `IRequestHandler<,>`.
- FluentValidation integrado ao pipeline do MediatR.
- Async/await em toda I/O (nunca `.Result` ou `.Wait()`).
- Nullable reference types habilitado — trate warnings como erros.
- Testes: xUnit + FluentAssertions + Moq para unitários; Testcontainers
  para integração com PostgreSQL real.

### 4.4 Não fazer

- `DateTime.Now` — usar `DateTime.UtcNow` ou injetar `IDateTimeProvider`.
- `Guid.NewGuid()` direto — injetar `IGuidProvider` quando testabilidade
  importar.
- Lógica de negócio em controllers/handlers — vai para o domínio.
- Retornar entidades de domínio diretamente da API — usar DTOs.
- Expor `DbContext` fora da Infrastructure.
- `catch (Exception)` genérico sem re-lançar ou logar.
- Strings mágicas — usar enums ou constantes.
- Misturar responsabilidades entre camadas.
- API Gateway, Nginx, proxy reverso ou service mesh.

## 5. Modelo de dados

Schema: `sigis`. Detalhamento completo pretendido em
[SIGIS-MESTRE.md](./SIGIS-MESTRE.md), seção 6.

### 5.1 PESSOA (núcleo de identidade canônica)

| Campo | Tipo | Observação |
|---|---|---|
| id | uuid PK | |
| nome_completo | string | |
| nome_normalizado | string | `unaccent+lower`, coluna gerada, índice GIN |
| data_nascimento | date | |
| cns | string | criptografado (pgcrypto) |
| cpf | string | criptografado (pgcrypto) |
| nome_mae | string | |
| sexo | string | |
| cor_raca | string | |
| telefone | string | |
| endereco | string | |
| bairro | string | |
| municipio_estado | string | |
| criado_em | timestamp | |

### 5.2 RESPONSAVEL

`id`, `pessoa_id` (FK), `nome`, `cns`, `data_nascimento`, `parentesco`.

### 5.3 UNIDADE_SERVICO

`id`, `nome`, `sigla`, `secretaria_responsavel` (`SAUDE | EDUCACAO | ASSISTENCIA`).

### 5.4 PROFISSIONAL

`id`, `nome`, `especialidade`, `unidade_id` (FK), `papel_rbac`
(`PROFISSIONAL | COORDENADOR | AUDITOR`).

### 5.5 FILA_ATENDIMENTO

`id`, `pessoa_id` (FK), `unidade_id` (FK), `especialidade`, `prioridade`
(`URGENTE | CURTO_PRAZO | LISTA_ESPERA`), `entrada_fila`, `status`
(`AGUARDANDO | EM_ATENDIMENTO | CONCLUIDO | FALTOU | BUSCA_ATIVA`).

### 5.6 ATENDIMENTO

`id`, `pessoa_id` (FK), `unidade_id` (FK), `profissional_id` (FK),
`data_hora`, `tipo_sessao` (`ANAMNESE_PSI | ANAMNESE_PSICOPED |
INSTRUMENTAL_EDFISICA | PRONTUARIO_NASF | SINTESE`), `comparecimento`
(`AGENDADO | COMPARECEU | FALTOU`), `dados_formulario` (jsonb — campos
específicos por serviço), `session_number`.

### 5.7 ENCAMINHAMENTO

`id`, `pessoa_id` (FK), `unidade_origem_id` (FK), `unidade_destino_id`
(FK), `motivo`, `prioridade`, `data_encaminhamento`, `status`
(`PENDENTE | ACEITO | CONCLUIDO | RECUSADO`).

### 5.8 ALERTA_DUPLICIDADE

`id`, `pessoa_id_1` (FK), `pessoa_id_2` (FK), `score_similaridade` (float),
`status` (`PENDENTE | MESCLADO | FALSO_POSITIVO`),
`resolvido_por_profissional_id` (FK), `resolvido_em`.

### 5.9 LOG_ACESSO

`id`, `pessoa_id` (FK), `profissional_id` (FK), `acao`, `base_legal`,
`data_hora`. Ver [lgpd.md](./lgpd.md).

### 5.10 Por que `dados_formulario` é JSONB

NAPE tem cinco fichas diferentes (anamnese psicológica, psicopedagógica,
instrumental de educação física, síntese de acompanhamento) e o NASF tem
ficha de prontuário totalmente distinta. Modelar isso como colunas fixas
em ATENDIMENTO geraria tabela com dezenas de colunas nulas na maioria das
linhas — prática ruim e frágil: qualquer novo campo exigiria migração de
schema.

**Solução:** `dados_formulario` como JSONB, com `tipo_sessao` referenciando
um template (JSON estático no frontend para o hackathon). PostgreSQL
permite indexar e consultar dentro de JSONB com operadores nativos (`->>`,
`@>`), então isso **não** compromete a geração de indicadores (RF11).

### 5.11 Extensões PostgreSQL e índices

```sql
-- Migration inicial:
CREATE EXTENSION IF NOT EXISTS pg_trgm;
CREATE EXTENSION IF NOT EXISTS unaccent;
CREATE EXTENSION IF NOT EXISTS pgcrypto;

CREATE SCHEMA IF NOT EXISTS sigis;

-- Coluna normalizada em PESSOA:
ALTER TABLE sigis.pessoa
  ADD COLUMN nome_normalizado text
  GENERATED ALWAYS AS (unaccent(lower(nome_completo))) STORED;

-- Índice GIN para busca fuzzy:
CREATE INDEX idx_pessoa_nome_trgm
  ON sigis.pessoa USING gin (nome_normalizado gin_trgm_ops);

-- Índice para filtro por data de nascimento:
CREATE INDEX idx_pessoa_data_nascimento
  ON sigis.pessoa (data_nascimento);

-- Índice único parcial (RN01 — uma pessoa não pode estar duas vezes
-- ativa na fila da mesma unidade):
CREATE UNIQUE INDEX idx_fila_ativa_unica
  ON sigis.fila_atendimento (pessoa_id, unidade_id)
  WHERE status IN ('AGUARDANDO', 'EM_ATENDIMENTO');
```

## 6. Enums do domínio

```
PrioridadeFila:            URGENTE | CURTO_PRAZO | LISTA_ESPERA
StatusFila:                AGUARDANDO | EM_ATENDIMENTO | CONCLUIDO | FALTOU | BUSCA_ATIVA
StatusComparecimento:      AGENDADO | COMPARECEU | FALTOU
TipoSessao:                ANAMNESE_PSI | ANAMNESE_PSICOPED | INSTRUMENTAL_EDFISICA | PRONTUARIO_NASF | SINTESE
StatusEncaminhamento:      PENDENTE | ACEITO | CONCLUIDO | RECUSADO
StatusAlertaDuplicidade:   PENDENTE | MESCLADO | FALSO_POSITIVO
PapelRbac:                 PROFISSIONAL | COORDENADOR | AUDITOR
SecretariaResponsavel:     SAUDE | EDUCACAO | ASSISTENCIA
```

## 7. Decisões-chave de design (não questionar)

Estas decisões foram tomadas com base no diagnóstico da Secretaria de
Saúde e nas fichas atuais dos serviços. Não devem ser revertidas sem
justificativa forte.

1. **Identidade canônica separada de registro de atendimento** — NASF e
   NAPE têm fichas completamente diferentes; forçar um prontuário único
   de campos fixos geraria tabela frágil e cheia de nulos. `PESSOA` é o
   núcleo compartilhado; `ATENDIMENTO` referencia `PESSOA` e guarda
   campos específicos de cada serviço.
2. **`ATENDIMENTO.dados_formulario` é JSONB** — ver seção 5.10.
3. **Deduplicação em 3 camadas — nunca automática** (ver
   [fluxos.md](./fluxos.md) para o fluxo completo e SQL de referência):
   1. Match exato por CNS — confiança máxima, mas merge ainda passa por
      revisão humana.
   2. Match exato por CPF — quando CNS ausente (comum no NAPE).
   3. Match probabilístico — `pg_trgm` com `unaccent` + `lower` + filtro
      duro por data de nascimento + limiar 0.75.

   **Regra crítica:** nunca fundir cadastros automaticamente. Sempre
   gravar `ALERTA_DUPLICIDADE` e deixar o coordenador decidir.
4. **RBAC com 3 papéis** — `PROFISSIONAL` (escopo: própria unidade),
   `COORDENADOR` (escopo: rede completa, resolve merges), `AUDITOR`
   (escopo: somente leitura de logs).
5. **LGPD by design** — ver [lgpd.md](./lgpd.md).
6. **Monolito modular, não microsserviços** — ver seção 3.3.
7. **Sem API Gateway** — ver seção 3.2.
