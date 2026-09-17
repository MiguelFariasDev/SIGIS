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

- Flutter 3.27+ — https://flutter.dev/



Subir os 3 containers:

make up



Sobe: sigis-db (PostgreSQL 16), sigis-api (.NET 10, porta 5000) e sigis-front (React 19 + Vite, porta 5173).



Rodar migrations:

make migrate



Popular com dados de demonstração:

make seed



Popula: 5 unidades (NASF, CREAES, NAPE, Casa Mais Azul, CRASF), 3 profissionais (um por papel), 10 pessoas — incluindo 2 gêmeas de dados (mesmo nome + mesma data de nascimento, em unidades diferentes) para demonstrar a deduplicação ao vivo.



Rodar testes:

make test # backend + frontend + mobile

make test-back # só backend

make test-front # só frontend

make test-mobile # só mobile



Encerrar:

make down



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