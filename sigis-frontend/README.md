# SIGIS — Frontend

Sistema Integrado de Gestao e Informacao em Saude — painel web (React 19 +
Vite + TypeScript) para a rede de cuidado a pessoas com TEA em Crateus/CE.

O backend (.NET 10 + PostgreSQL) ainda esta em construcao. Enquanto isso, o
frontend roda inteiramente sobre dados simulados via **MSW (Mock Service
Worker)** — todas as 12 telas do fluxo sao navegaveis com esses mocks.

## Stack

React 19 · Vite · TypeScript · Tailwind CSS 4 · shadcn/ui (canary) ·
TanStack Query v5 · React Router v7 · Axios · Zod · react-hook-form ·
date-fns · MSW · Sonner · Zustand · @dnd-kit

## Rodando localmente

```bash
npm install
cp .env.example .env
npm run dev
```

Acesse `http://localhost:5173`. Use qualquer senha com um dos e-mails de
demonstracao exibidos na tela de login (um por papel: PROFISSIONAL,
COORDENADOR, AUDITOR).

## Mocks

Com `VITE_USE_MOCKS=true` (padrao), todas as chamadas HTTP sao interceptadas
pelo MSW (`src/lib/mocks`), com dados em memoria incluindo um par de
cadastros "gemeos" (duplicidade pendente) e uma pessoa com historico em 3
unidades (timeline unificada). Quando o backend real estiver disponivel,
defina `VITE_USE_MOCKS=false` e ajuste `VITE_API_URL`.

## Scripts

- `npm run dev` — servidor de desenvolvimento
- `npm run build` — type-check (`tsc -b`) + build de producao
- `npm run lint` — oxlint
- `npm run preview` — preview do build de producao

## Estrutura

```
src/
├── components/{ui,layout,common,domain}
├── features/{auth,dashboard,pacientes,duplicidades,filas,atendimentos,encaminhamentos,auditoria}
├── lib/{api,types,utils,mocks}
├── stores/
└── styles/theme.css   — paleta SIGIS (placeholder, nao usa o simbolo oficial do SUS)
```
