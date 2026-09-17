# SIGIS — Sistema Integrado de Gestão e Informação em Saúde

> Plataforma intersetorial para gestão do cuidado a pessoas com Transtorno do
> Espectro Autista (TEA) na rede pública de Crateús/CE.

[![.NET 10](https://img.shields.io/badge/.NET-10-512BD4)](https://dotnet.microsoft.com/)
[![PostgreSQL 16](https://img.shields.io/badge/PostgreSQL-16-336791)](https://www.postgresql.org/)
[![React 19](https://img.shields.io/badge/React-19-61DAFB)](https://react.dev/)
[![Flutter](https://img.shields.io/badge/Flutter-3.27+-02569B)](https://flutter.dev/)

---

## O problema

A rede pública municipal de Crateús — NASF, CREAES, NAPE, Casa Mais Azul e
CRASF — atende pessoas com TEA hoje com quatro dores concretas:

1. Não existe sistema informatizado para gerir a inserção e o acompanhamento
   desses pacientes na rede.
2. Não há detecção de duplicidade de atendimento entre os serviços — o mesmo
   paciente pode estar em três filas diferentes sem ninguém saber.
3. Filas e histórico em cadernos físicos, sem rastreabilidade.
4. Informações fragmentadas entre serviços, comprometendo a continuidade do
   cuidado.

Esse diagnóstico foi feito pela própria Secretaria Municipal de Saúde e é o
ponto de partida do hackathon.

---

## A solução

O SIGIS resolve as quatro dores com quatro pilares técnicos:

| Pilar | O que faz |
|---|---|
| Cadastro único | Pessoa como núcleo canônico de identidade, com deduplicação em 3 camadas (CNS → CPF → similaridade pg_trgm) |
| Fila multidisciplinar | Visível, priorizável por serviço e por especialidade, respeitando o escopo de cada unidade |
| Linha do tempo federada | Histórico consolidado entre Saúde, Educação e Assistência, com metadados por padrão e conteúdo sob consentimento |
| Compartilhamento LGPD | Consentimento granular por tipo, base legal explícita, auditoria antes do acesso cross-secretaria |

### Diferenciais

- Intersetorial de verdade — cobre as duas facetas da pessoa: paciente
  (Saúde) e aluno (Educação). As fichas do NASF e do NAPE são radicalmente
  diferentes, e o SIGIS preserva cada uma sem forçar prontuário único
  genérico.
- JSONB para formulários por serviço — cada unidade mantém sua ficha própria
  (anamnese psicológica, psicopedagógica, instrumental de educação física,
  síntese) sem exigir migração de schema a cada novo campo.
- Rastreio de fluxo entre unidades — encaminhamento NASF → NAPE com timeline
  visual: quem encaminhou, quando, quem aceitou, quando atendeu.
- Mobile offline-first — o agente de saúde registra check-in e sessão mesmo
  sem internet; o app sincroniza quando a conexão volta.

---

## Arquitetura

Monolito modular em Clean Architecture (mesmo padrão do sistema Iustitia, já
usado pela SEPLATI). Sem microsserviços, sem API Gateway — decisão consciente
para um hackathon de 20 horas com verba pública municipal.

┌─────────────────────────────────────────────────────────────────┐
│  CLIENTES                                                       │
│  React SPA (gestor/prof)  ·  Flutter (campo)  ·  Auditor/DPO    │
└────────────┬─────────────────────┬──────────────────┬───────────┘
             │ HTTPS/JWT           │ HTTPS/JWT        │
             ▼                     ▼                  ▼
┌─────────────────────────────────────────────────────────────────┐
│  Sigis.Api (ASP.NET Core 10) — monolito modular                 │
│  ┌───────────┬───────────┬───────────┬────────────────────┐    │
│  │ Cadastro  │ Fila      │ Atendim.  │ Deduplicação       │    │
│  │ Único     │           │ Histórico │                    │    │
│  └───────────┴───────────┴───────────┴────────────────────┘    │
│  MediatR · FluentValidation · JWT · CORS · Serilog              │
└────────────┬────────────────────────────────────────────────────┘
             ▼
       ┌─────────────┐
       │ PostgreSQL  │  + pg_trgm  + unaccent  + pgcrypto
       │     16      │
       └─────────────┘

### Camadas (dependências fluem para dentro)

Sigis.Api            ← Controllers, Minimal APIs, middlewares
    ↓
Sigis.Infrastructure ← EF Core, repositórios, matching pg_trgm
    ↓
Sigis.Application    ← Casos de uso (CQRS + MediatR), validators
    ↓
Sigis.Domain         ← Entidades, VOs, regras de negócio puras

Domain não conhece ninguém. Application conhece só Domain. Infrastructure
conhece Application e Domain. Api conhece Application e Infrastructure.

---

## Stack

| Camada | Tecnologias |
|---|---|
| Backend | .NET 10 · ASP.NET Core · Clean Architecture · MediatR · FluentValidation · AutoMapper · EF Core 10 |
| Banco | PostgreSQL 16 · pg_trgm · unaccent · pgcrypto · schema sigis |
| Frontend | React 19 · Vite 6 · TypeScript 5.7 · TanStack Query v5 · Tailwind CSS 4 · shadcn/ui · Zod |
| Mobile | Flutter 3.27+ · Riverpod 2 · Dio · sqflite · offline-first |
| Infra | Docker Compose · GitHub Actions |
| Observabilidade | Serilog (structured logging) |

---

## LGPD e governança de dados

Dado de saúde é dado sensível (LGPD, art. 5º, II). O SIGIS implementa:

- Consentimento granular por tipo: Clinical, Educational, SocialAssistance,
  Research — cada um com data, evidência e versão do termo.
- Base legal explícita gravada em log_acesso (art. 11, II, LGPD) — nunca em
  branco, nunca como checkbox decorativo.
- Auditoria antes do acesso — acesso cross-secretaria é registrado antes de
  retornar os dados, não depois.
- Minimização — a timeline mostra apenas metadados por padrão; conteúdo
  clínico completo de outra secretaria exige justificativa ≥ 50 caracteres.
- Criptografia em repouso para CNS e CPF via pgcrypto.
- RBAC de 3 papéis: Profissional (própria unidade), Coordenador (rede
  completa + merge), Auditor (somente leitura de logs).

### Compartilhamento entre Secretarias

| Cruzamento | Base legal | Precisa consentimento específico? |
|---|---|---|
| Saúde ↔ Saúde | Art. 11, II, "f" — tutela da saúde | Não |
| Saúde ↔ Assistência | Art. 11, II, "d" — proteção social | Sim |
| Saúde ↔ Educação | Art. 7º, III + Art. 11, II, "b" | Sim, sempre |

O cruzamento Saúde ↔ Educação (NASF/NAPE) é o mais sensível e tem tratamento
diferenciado no código.

---

## Como subir

### Pré-requisitos

- .NET 10 SDK — https://dotnet.microsoft.com/
- Node.js 22 — https://nodejs.org/
- Docker + Docker Compose — https://www.docker.com/
- Flutter 3.27+ — https://flutter.dev/

### Subir os 3 containers

    make up

Sobe: sigis-db (PostgreSQL 16), sigis-api (.NET 10, porta 5000) e sigis-front
(React 19 + Vite, porta 5173).

### Rodar migrations

    make migrate

### Popular com dados de demonstração

    make seed

Popula: 5 unidades (NASF, CREAES, NAPE, Casa Mais Azul, CRASF), 3
profissionais (um por papel), 10 pessoas — incluindo 2 gêmeas de dados (mesmo
nome + mesma data de nascimento, em unidades diferentes) para demonstrar a
deduplicação ao vivo.

### Rodar testes

    make test          # backend + frontend + mobile
    make test-back     # só backend
    make test-front    # só frontend
    make test-mobile   # só mobile

### Encerrar

    make down

---

## Estrutura

sigis/
├── docker-compose.yml
├── Makefile
├── docs/
│   ├── requisitos.md
│   ├── system-design.md
│   ├── fluxos.md
│   ├── lgpd.md
│   └── governanca-dados.md
├── sigis-backend/           ← .NET 10 — Clean Architecture
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
├── sigis-frontend/          ← React  + Vite  + TS
│   └── src/
│       ├── features/
│       ├── components/
│       ├── lib/api/
│       ├── routes/
│       └── templates/
└── sigis-mobile/            ← Flutter 
    └── lib/
        ├── features/
        ├── data/
        └── core/

---

## Documentação

| Documento | Conteúdo |
|---|---
| docs/lgpd.md | Mecanismo de compartilhamento e conformidade |
| docs/governanca-dados.md | Governança entre Secretarias |

---

## Roadmap

- [x] Bootstrap (monorepo, Docker, CI)
- [x] Modelo de domínio (entidades, VOs, enums, erros agrupados)
- [x] Persistência (DbContext, migrations, extensões PostgreSQL)
- [x] Autenticação JWT + RBAC (3 papéis)
- [x] Cadastro único com deduplicação em 3 camadas
- [x] Fila, atendimento e encaminhamento
- [x] Timeline federada + compartilhamento LGPD
- [x] Frontend web (12 telas + 4 fluxos críticos)
- [x] Mobile offline-first (Android)
- [ ] Seed de demonstração expandido
- [ ] Roteiro de pitch

---

## Contexto

Desenvolvido para o Hackathon Banco do Nordeste · UFC · Prefeitura de
Crateús, a partir de necessidade identificada pela Secretaria Municipal de
Saúde e proposta técnica da SEPLATI — Secretaria Municipal de Planejamento e
Tecnologia da Informação.

