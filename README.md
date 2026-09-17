# SIGIS — Sistema Integrado de Gestão e Informação em Saúde

### Gestão do cuidado a pessoas com TEA — Crateús/CE

![CI](https://github.com/SEU_ORG/SEU_REPO/actions/workflows/ci.yml/badge.svg)

Sistema para a Secretaria Municipal de Saúde de Crateús/CE gerenciar o cuidado
a pessoas com Transtorno do Espectro Autista (TEA) na rede pública, unificando
cadastro, fila de atendimento, histórico e deduplicação entre NASF, CREAES,
NAPE, Casa Mais Azul e CRASF.

## Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/)
- [Node.js 22](https://nodejs.org/)
- [Docker](https://www.docker.com/) + Docker Compose
- [Flutter 3.27+](https://flutter.dev/)

## Como subir

```bash
make up
```

Sobe os 3 containers obrigatórios: `sigis-db` (PostgreSQL 16), `sigis-api`
(.NET 10) e `sigis-front` (React 19 + Vite).

## Como rodar migrations

```bash
make migrate
```

## Como rodar testes

```bash
make test          # backend + frontend + mobile
make test-back      # só backend
make test-front     # só frontend
make test-mobile    # só mobile
```

## Estrutura

```
sigis/
├── docker-compose.yml
├── Makefile
├── docs/
│   └── SIGIS-MESTRE.md
├── sigis-backend/       # .NET 10 — Clean Architecture
│   ├── Sigis.sln
│   ├── src/
│   │   ├── Sigis.Domain/
│   │   ├── Sigis.Application/
│   │   ├── Sigis.Infrastructure/
│   │   ├── Sigis.Api/
│   │   └── Sigis.Mobile.Contracts/
│   └── tests/
├── sigis-frontend/      # React 19 + Vite 6 + TS
└── sigis-mobile/        # Flutter 3.27+
```

Documentação completa em [docs/SIGIS-MESTRE.md](docs/SIGIS-MESTRE.md)
