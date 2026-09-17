SIGIS — Sistema Integrado de Gestão e Informação em Saúde



Plataforma intersetorial para gestão do cuidado a pessoas com Transtorno do Espectro Autista (TEA) na rede pública de Crateús/CE.



Tecnologias: .NET 10 | PostgreSQL 16 | React 19 | Flutter 3.27+



--------------------------------------------------------------------------------



O PROBLEMA



A rede pública municipal de Crateús — NASF, CREAES, NAPE, Casa Mais Azul e CRASF — atende pessoas com TEA hoje com quatro dores concretas:



1. Não existe sistema informatizado para gerir a inserção e o acompanhamento desses pacientes na rede.

2. Não há detecção de duplicidade de atendimento entre os serviços — o mesmo paciente pode estar em três filas diferentes sem ninguém saber.

3. Filas e histórico em cadernos físicos, sem rastreabilidade.

4. Informações fragmentadas entre serviços, comprometendo a continuidade do cuidado.



Esse diagnóstico foi feito pela própria Secretaria Municipal de Saúde e é o ponto de partida do hackathon.



A SOLUÇÃO



O SIGIS resolve as quatro dores com quatro pilares técnicos:



| Pilar                   | O que faz                                                                                                        |
| ----------------------- | ---------------------------------------------------------------------------------------------------------------- |
| Cadastro único          | Pessoa como núcleo canônico de identidade, com deduplicação em 3 camadas (CNS → CPF → similaridade pg_trgm)      |
| Fila multidisciplinar   | Visível, priorizável por serviço e por especialidade, respeitando o escopo de cada unidade                       |
| Linha do tempo federada | Histórico consolidado entre Saúde, Educação e Assistência, com metadados por padrão e conteúdo sob consentimento |
| Compartilhamento LGPD   | Consentimento granular por tipo, base legal explícita, auditoria antes do acesso cross-secretaria                |



Diferenciais:



- Intersetorial de verdade — cobre as duas facetas da pessoa: paciente (Saúde) e aluno (Educação). As fichas do NASF e do NAPE são radicalmente diferentes, e o SIGIS preserva cada uma sem forçar prontuário único genérico.

- JSONB para formulários por serviço — cada unidade mantém sua ficha própria (anamnese psicológica, psicopedagógica, instrumental de educação física, síntese) sem exigir migração de schema a cada novo campo.

- Rastreio de fluxo entre unidades — encaminhamento NASF → NAPE com timeline visual: quem encaminhou, quando, quem aceitou, quando atendeu.

- Mobile offline-first — o agente de saúde registra check-in e sessão mesmo sem internet; o app sincroniza quando a conexão volta.



--------------------------------------------------------------------------------



## Arquitetura

O SIGIS é construído como um **monolito modular** seguindo os princípios da
**Clean Architecture**, com separação clara de responsabilidades entre as
camadas. Cada módulo de domínio (Cadastro, Fila, Atendimento, Deduplicação)
é isolado dentro do mesmo processo e comunica-se com os demais por meio de
**eventos** e do padrão **CQRS** com **MediatR**.

### Por que monolito modular

A escolha por monolito modular — em vez de microsserviços ou arquitetura
distribuída — é consciente e alinhada ao contexto do projeto:

- **Custo de operação baixo** — um único processo para hospedar, uma única
  base de dados para manter, sem orquestração distribuída.
- **Adequado ao porte do problema** — a rede municipal de saúde de Crateús
  atende uma população de porte médio; não há necessidade de escala
  horizontal nem de resiliência distribuída.
- **Facilita a manutenção** — toda a base de código fica em um único
  repositório, com deploy único e rollback simples.
- **Permite evolução futura** — se em algum momento um módulo específico
  precisar ser extraído como serviço separado, isso é possível sem reescrever
  o domínio.

### Padrões arquiteturais adotados

- **Clean Architecture** — separação em quatro camadas com dependências
  fluindo sempre de fora para dentro.
- **CQRS** — comandos (escrita) e consultas (leitura) são separados em
  casos de uso distintos, cada um com seu handler dedicado.
- **MediatR** — comunicação entre controllers e casos de uso por meio de
  mensagens, sem acoplamento direto.
- **Result Pattern** — casos de uso retornam `Result<T>` em vez de lançar
  exceções para fluxos esperados, deixando o controle de erros explícito.
- **Domain-Driven Design** — entidades ricas com comportamento, value
  objects imutáveis, eventos de domínio e linguagem ubíqua alinhada ao
  domínio da saúde pública.
- **Repository Pattern** — abstração do acesso a dados por interfaces
  definidas no domínio e implementadas na infraestrutura.
- **Dependency Injection** — inversão de dependências nativa do .NET,
  garantindo baixo acoplamento entre as camadas.

### Camadas do projeto

O código está organizado em quatro camadas principais, com dependências
fluindo sempre de fora para dentro:

- **`Sigis.Domain`** — entidades, value objects, enums, eventos de domínio,
  erros agrupados e regras de negócio puras. Não depende de nenhuma outra
  camada nem de bibliotecas externas.

- **`Sigis.Application`** — casos de uso organizados por módulo, handlers
  do MediatR, validators com FluentValidation, DTOs, mapeamentos e pipeline
  behaviors. Depende apenas de `Sigis.Domain`.

- **`Sigis.Infrastructure`** — implementação concreta do acesso a dados com
  EF Core, repositórios, `DbContext`, migrations, serviços de matching
  (pg_trgm), criptografia e integrações externas. Depende de
  `Sigis.Application` e `Sigis.Domain`.

- **`Sigis.Api`** — exposição HTTP via Controllers MVC, middlewares,
  configuração de autenticação JWT, autorização por políticas (RBAC) e
  Swagger. Depende de `Sigis.Application` e `Sigis.Infrastructure`.

### Fluxo de uma requisição

1. O cliente (React, Flutter ou auditor) envia uma requisição HTTP para a
   API com o token JWT no header.
2. O middleware de autenticação valida o token e popula o contexto do
   usuário (profissional, unidade, papel).
3. A controller recebe a requisição e envia um comando ou consulta via
   MediatR.
4. O pipeline behavior executa a validação (FluentValidation) antes de
   chamar o handler.
5. O handler orquestra o caso de uso, consultando o domínio e os
   repositórios.
6. A infraestrutura persiste no PostgreSQL, aplicando as regras de negócio
   no domínio e gravando eventos de domínio.
7. O `Result<T>` é convertido em resposta HTTP apropriada (200, 201, 400,
   404, 409) pela classe base dos controllers.
8. Se houver acesso cross-secretaria, um log de auditoria é gravado **antes**
   de retornar os dados ao cliente.


--------------------------------------------------------------------------------



STACK



| Camada          | Tecnologias                                                                                        |
| --------------- | -------------------------------------------------------------------------------------------------- |
| Backend         | .NET 10 · ASP.NET Core · EF Core 10 |
| Banco           | PostgreSQL 16                                        |
| Frontend        | React 19 · Vite 6 · TypeScript 5.7 ·          |
| Mobile          | Flutter 3.27+ · Riverpod 2 · Dio · sqflite ·                                        |
| Infra           | Docker Compose · GitHub Actions                                                                    |
| Observabilidade | Serilog (structured logging)                                                                       |



--------------------------------------------------------------------------------



LGPD E GOVERNANÇA DE DADOS



Dado de saúde é dado sensível (LGPD, art. 5º, II). O SIGIS implementa:



- Consentimento granular por tipo: Clinical, Educational, SocialAssistance, Research — cada um com data, evidência e versão do termo.

- Base legal explícita gravada em log_acesso (art. 11, II, LGPD) — nunca em branco, nunca como checkbox decorativo.

- Auditoria antes do acesso — acesso cross-secretaria é registrado antes de retornar os dados, não depois.

- Minimização — a timeline mostra apenas metadados por padrão; conteúdo clínico completo de outra secretaria exige justificativa ≥ 50 caracteres.

- Criptografia em repouso para CNS e CPF via pgcrypto.

- RBAC de 3 papéis: Profissional (própria unidade), Coordenador (rede completa + merge), Auditor (somente leitura de logs).



Compartilhamento entre Secretarias:



| Cruzamento          | Base legal                         | Precisa consentimento específico? |
| ------------------- | ---------------------------------- | --------------------------------- |
| Saúde ↔ Saúde       | Art. 11, II, "f" — tutela da saúde | Não                               |
| Saúde ↔ Assistência | Art. 11, II, "d" — proteção social | Sim                               |
| Saúde ↔ Educação    | Art. 7º, III + Art. 11, II, "b"    | Sim, sempre                       |



O cruzamento Saúde ↔ Educação (NASF/NAPE) é o mais sensível e tem tratamento diferenciado no código.



--------------------------------------------------------------------------------



COMO SUBIR



Pré-requisitos:

- .NET 10 SDK — https://dotnet.microsoft.com/

- Node.js 22 — https://nodejs.org/

- Docker + Docker Compose — https://www.docker.com/

- Flutter 3.27+ — https://flutter.dev/ (só necessário para o app mobile)



### Opção 1 — tudo em Docker (um comando)

make up      # sobe sigis-db (5435), sigis-api (5000) e sigis-front (5173)

make migrate # roda as migrations do EF Core dentro do container

make down    # encerra os containers



### Opção 2 — passo a passo manual (3 abas de terminal)

Mais prático para desenvolver: o banco fica em container, mas o backend e o
frontend rodam direto na máquina (hot reload, debug, etc.). Depois de clonar
o repositório, abra 3 abas de terminal na raiz do projeto:

**Aba 1 — Banco de dados (Docker)**

docker compose up -d sigis-db

Sobe só o PostgreSQL 16 (extensões `pg_trgm`, `unaccent`, `pgcrypto` já
aplicadas via `docker/init-extensions.sql`), exposto em `localhost:5435`.
Confirme que subiu com `docker compose ps` (status `healthy`).

**Aba 2 — Backend (.NET)**

cd sigis-backend

dotnet tool install --global dotnet-ef   # só na primeira vez

dotnet ef database update \

  --project src/Sigis.Infrastructure \

  --startup-project src/Sigis.Api

dotnet run --project src/Sigis.Api --urls http://localhost:5000

O `appsettings.Development.json` já aponta para `localhost:5435` (o banco
da Aba 1), então nenhuma configuração extra é necessária. A API sobe em
`http://localhost:5000` e o Swagger fica disponível em
`http://localhost:5000/swagger` (só em ambiente Development).

**Aba 3 — Frontend (React + Vite)**

cd sigis-frontend

npm install

npm run dev

Sobe em `http://localhost:5173`. O arquivo `.env.local` já existe com
`VITE_API_URL=http://localhost:5000` e `VITE_USE_MOCKS=false` — garanta que
`VITE_USE_MOCKS` esteja `false` para consumir a API real em vez dos mocks
(MSW) usados nos testes/Storybook.



### Popular o banco com dados de demonstração

Não é preciso rodar um script `.sql` à parte: assim que o backend sobe em
ambiente Development (Aba 2, `dotnet run`), o `DevelopmentSeeder` roda
automaticamente e popula o banco **se ele ainda estiver vazio** (é
idempotente — reiniciar a API não duplica os dados). Isso evita que um dump
`.sql` fique desatualizado a cada nova migration.

O seed cria: 5 unidades (NASF, CREAES/NAPE, Casa Mais Azul, CRASF), 5
profissionais (um por papel/unidade), 12 pessoas — incluindo casos prontos
para demonstração (2 "gêmeas" de cadastro para a deduplicação, timeline
federada, consentimento parcial, encaminhamento aceito), filas, atendimentos,
1 alerta de duplicidade pendente e logs de auditoria.

Se preferir disparar o seed manualmente (fora do fluxo automático), use:

make seed   # equivale a: dotnet run --project sigis-backend/src/Sigis.Api -- --seed

**Credenciais de demonstração** (senha igual para todos: `Senha123!`):

| Papel (RBAC)  | E-mail                     | Unidade       |
| ------------- | --------------------------- | ------------- |
| Coordenador   | coordenador@sigis.gov.br    | NASF          |
| Profissional  | psicologo@sigis.gov.br      | NAPE          |
| Profissional  | fono@cma.gov.br             | Casa Mais Azul|
| Profissional  | assistente@crasf.gov.br     | CRASF         |
| Auditor       | auditor@sigis.gov.br        | NASF          |

Login: `POST http://localhost:5000/api/auth/login` com `{"email": "...", "password": "Senha123!"}` — o token JWT retornado deve ser usado no header `Authorization: Bearer <token>` nas demais chamadas (ou colado direto no botão "Authorize" do Swagger).



Rodar testes:

make test # backend + frontend + mobile

make test-back # só backend

make test-front # só frontend

make test-mobile # só mobile



--------------------------------------------------------------------------------



## ENDPOINTS DA API



Base URL local: `http://localhost:5000`. Todos os endpoints (exceto login)
exigem o header `Authorization: Bearer <token>` obtido em `POST
/api/auth/login`. Corpo de erro padrão: `{"code": "...", "description": "...", "type": "..."}`
— o status HTTP é sempre derivado do `type` do erro:
`NotFound → 404`, `Conflict → 409`, `Unauthorized → 401`, `Forbidden → 403`,
`Internal → 500`, qualquer outro (validação) `→ 400`.

Papéis RBAC: **Professional** (própria unidade), **Coordinator** (rede
completa + merge/deduplicação), **Auditor** (somente leitura de auditoria).
Onde não houver política específica indicada, o endpoint aceita qualquer
usuário autenticado (`RequireAuthenticated`).

### Autenticação (`/api/auth`)

| Método | Rota | Auth | Body | Respostas |
| --- | --- | --- | --- | --- |
| POST | `/api/auth/login` | Público | `{ email, password }` | 200 (token), 400, 401, 403 |
| GET | `/api/auth/me` | Autenticado | — | 200 (dados do profissional logado), 401 |

### Pessoas — Cadastro único (`/api/pessoas`)

| Método | Rota | Auth | Body | Respostas |
| --- | --- | --- | --- | --- |
| POST | `/api/pessoas` | Autenticado | `CreatePersonRequest` (nome, nascimento, CNS/CPF opcionais, endereço opcional...) | 201 (criado sem suspeita de duplicidade), 200 (criado + candidatos a duplicidade), 400, 409 (CNS/CPF já existente) |
| GET | `/api/pessoas/busca?termo=&limit=` | Autenticado | — | 200 (lista, pode ser vazia) |
| GET | `/api/pessoas/{id}` | Autenticado | — | 200, 404 |
| GET | `/api/pessoas/{id}/linha-do-tempo?nivel=&justificativa=` | Autenticado | — | 200, 403 (justificativa obrigatória e não informada), 404 |
| POST | `/api/pessoas/mesclar` | **Coordinator** | `{ sourcePersonId, targetPersonId, duplicateAlertId? }` | 200, 400 (origem = destino), 404 |
| POST | `/api/pessoas/{id}/solicitar-acesso` | Autenticado | `{ justificativa }` | 200, 400, 404 |

### Fila de atendimento (`/api/filas`, `/api/unidades/{unidadeId}/fila`)

| Método | Rota | Auth | Body | Respostas |
| --- | --- | --- | --- | --- |
| GET | `/api/unidades/{unidadeId}/fila?status=&prioridade=&especialidade=` | Autenticado | — | 200, 404 (unidade não existe) |
| POST | `/api/filas/{id}/chamar` | Autenticado | — | 200, 404, 409 (transição de status inválida) |
| PATCH | `/api/filas/{id}/comparecimento` | Autenticado | `{ comparecimento: "COMPARECEU"\|"FALTOU" }` | 200, 400, 404, 409 (fila já concluída) |

### Atendimentos (`/api/atendimentos`)

| Método | Rota | Auth | Body | Respostas |
| --- | --- | --- | --- | --- |
| POST | `/api/atendimentos` | Autenticado | `RegisterAttendanceRequest` (personId, unitId, professionalId, dateTime, sessionType, formData?...) | 201, 400, 404 |
| PATCH | `/api/atendimentos/{id}/comparecimento` | Autenticado | `{ comparecimento, mainComplaint? }` | 200, 400, 404, 409 (comparecimento já registrado) |

### Encaminhamentos (`/api/encaminhamentos`)

| Método | Rota | Auth | Body | Respostas |
| --- | --- | --- | --- | --- |
| POST | `/api/encaminhamentos` | Autenticado | `{ personId, originUnitId, destinationUnitId, reason?, priority }` | 201, 400, 403, 404 |
| GET | `/api/encaminhamentos/recebidos` | Autenticado | — | 200 (da unidade do profissional logado), 403 |
| GET | `/api/encaminhamentos/enviados` | Autenticado | — | 200 (da unidade do profissional logado), 403 |
| GET | `/api/encaminhamentos/pessoa/{pessoaId}` | Autenticado | — | 200 |
| POST | `/api/encaminhamentos/{id}/aceitar` | Autenticado | — | 200, 404, 409 (não está pendente) |
| POST | `/api/encaminhamentos/{id}/recusar` | Autenticado | `{ motivo? }` | 200, 400, 404, 409 (não está pendente) |
| GET | `/api/encaminhamentos/{id}/rastreio` | Autenticado | — | 200, 404 |

### Duplicidades (`/api/duplicidades`)

| Método | Rota | Auth | Body | Respostas |
| --- | --- | --- | --- | --- |
| GET | `/api/duplicidades/pendentes?skip=&take=` | **Coordinator** | — | 200 (lista, pode ser vazia) |
| GET | `/api/duplicidades/{id}` | **Coordinator** | — | 200, 404 |
| POST | `/api/duplicidades/{id}/resolver` | **Coordinator** | `{ acao: "MESCLAR"\|"FALSO_POSITIVO" }` | 200, 400, 404, 409 (já resolvido) |
| POST | `/api/duplicidades/{id}/falso-positivo` | **Coordinator** | `{ note? }` | 204, 404, 409 (já resolvido) |

### Consentimentos LGPD (`/api/pessoas/{pessoaId}/consentimentos`)

| Método | Rota | Auth | Body | Respostas |
| --- | --- | --- | --- | --- |
| GET | `/api/pessoas/{pessoaId}/consentimentos` | Autenticado | — | 200, 404 |
| POST | `/api/pessoas/{pessoaId}/consentimentos` | Autenticado | `{ type, version, evidence?, grantedByGuardianId? }` | 201, 400, 404, 409 (já ativo do mesmo tipo) |
| DELETE | `/api/pessoas/{pessoaId}/consentimentos/{id}` | Autenticado | `{ justification }` | 204, 400, 404, 409 (já revogado) |

### Auditoria (`/api/auditoria`) — restrito ao papel **Auditor**

| Método | Rota | Auth | Body | Respostas |
| --- | --- | --- | --- | --- |
| GET | `/api/auditoria/acessos?pessoaId=&profissionalId=&dataInicio=&dataFim=` | **Auditor** | — | 200 |
| GET | `/api/auditoria/acessos-cross` | **Auditor** | — | 200 (acessos que cruzaram unidades/secretarias) |
| GET | `/api/auditoria/export-csv?pessoaId=&profissionalId=&dataInicio=&dataFim=` | **Auditor** | — | 200 (arquivo `.csv`, mesmos filtros de `/acessos`) |

### Indicadores (`/api/indicadores`)

| Método | Rota | Auth | Body | Respostas |
| --- | --- | --- | --- | --- |
| GET | `/api/indicadores/painel?unidadeId=&dataInicio=&dataFim=` | Autenticado | — | 200 (painel de fila/atendimentos/encaminhamentos) |

### Unidades de serviço (`/api/unidades`)

| Método | Rota | Auth | Body | Respostas |
| --- | --- | --- | --- | --- |
| GET | `/api/unidades` | Autenticado | — | 200 (lista completa, para seletores da UI) |

### Bases legais LGPD (`/api/legal-basis`) — conteúdo estático

| Método | Rota | Auth | Body | Respostas |
| --- | --- | --- | --- | --- |
| GET | `/api/legal-basis?secretariat=` | Autenticado | — | 200 |

### Ficha ampliada da pessoa (sub-recursos de `/api/pessoas/{pessoaId}/...`)

Cada um destes segue o mesmo padrão GET (consulta) / POST ou PUT (grava);
todos exigem `Autenticado` e retornam 404 quando a pessoa não existir.

| Método | Rota | Body | Respostas |
| --- | --- | --- | --- |
| GET / PUT | `/api/pessoas/{pessoaId}/perfil-clinico` | `{ medicalRecordNumber?, clinicalHypothesis?, apsReferenceUnitId? }` | 200, 400, 404, 409 (nº de prontuário duplicado na unidade) |
| GET / PUT | `/api/pessoas/{pessoaId}/composicao-familiar` | todos os campos opcionais (pai/mãe, irmãos, gestação...) | 200, 400, 404 |
| GET / PUT | `/api/pessoas/{pessoaId}/desenvolvimento` | todos os campos opcionais (marcos motores, dificuldades...) | 200, 400, 404 |
| GET / POST | `/api/pessoas/{pessoaId}/historico-escolar` | `{ schoolName, grade, schoolYear, shift?, classGroup?, startDate?, notes? }` | GET: 200, 404 · POST: 201, 400, 404, 409 (vínculo ativo já existe) |
| GET / POST | `/api/pessoas/{pessoaId}/dificuldades-aprendizagem` | `{ type, severity?, assessmentDate?, notes? }` | GET: 200, 404 · POST: 201, 400, 404, 409 (já registrado) |
| GET / POST / DELETE | `/api/pessoas/{pessoaId}/tratamentos-concomitantes` | `{ specialty, location, professionalName, dayOfWeek, startTime, endTime, notes? }` | GET: 200, 404 · POST: 201, 400, 404, 409 (sobreposição de horário) · DELETE `/{id}`: 204, 404 |



--------------------------------------------------------------------------------



ESTRUTURA



sigis/

├── docker-compose.yml

├── Makefile

├── docs/

│ ├── requisitos.md

│ ├── system-design.md

│ ├── fluxos.md

│ ├── lgpd.md

│ └── governanca-dados.md

├── sigis-backend/ ← .NET 10 — Clean Architecture

│ ├── Sigis.sln

│ ├── src/

│ │ ├── Sigis.Domain/

│ │ ├── Sigis.Application/

│ │ ├── Sigis.Infrastructure/

│ │ ├── Sigis.Api/

│ │ └── Sigis.Mobile.Contracts/

│ └── tests/

│ ├── Sigis.Domain.Tests/

│ └── Sigis.Application.Tests/

├── sigis-frontend/ ← React + Vite + TS

│ └── src/

│ ├── features/

│ ├── components/

│ ├── lib/api/

│ ├── routes/

│ └── templates/

└── sigis-mobile/ ← Flutter

└── lib/

├── features/

├── data/

└── core/



--------------------------------------------------------------------------------



DOCUMENTAÇÃO



| Documento                | Conteúdo                                     |
| ------------------------ | -------------------------------------------- |
| docs/lgpd.md             | Mecanismo de compartilhamento e conformidade |
| docs/governanca-dados.md | Governança entre Secretarias                 |



--------------------------------------------------------------------------------



ROADMAP



[x] Bootstrap (monorepo, Docker, CI)

[x] Modelo de domínio (entidades, VOs, enums, erros agrupados)

[x] Persistência (DbContext, migrations, extensões PostgreSQL)

[x] Autenticação JWT + RBAC (3 papéis)

[x] Cadastro único com deduplicação em 3 camadas

[x] Fila, atendimento e encaminhamento

[x] Timeline federada + compartilhamento LGPD

[x] Frontend web (12 telas + 4 fluxos críticos)

[x] Mobile offline-first (Android)

[ ] Seed de demonstração expandido

[ ] Roteiro de pitch



--------------------------------------------------------------------------------



CONTEXTO



Desenvolvido para o Hackathon Banco do Nordeste · UFC · Prefeitura de Crateús, a partir de necessidade identificada pela Secretaria Municipal de Saúde e proposta técnica da SEPLATI — Secretaria Municipal de Planejamento e Tecnologia da Informação.