# Documentação do SIGIS

Documentação técnica gerada a partir de `claude.md` (contexto persistente
do projeto). Cada documento referencia a seção de origem no `claude.md` e
sinaliza explicitamente quando algum conteúdo ainda não está disponível
no arquivo fonte.

- [requisitos.md](./requisitos.md) — contexto do problema, dores, personas
  e requisitos funcionais/não-funcionais (RF/RNF).
- [system-design.md](./system-design.md) — stack, estrutura do repositório,
  arquitetura (Clean Architecture, sem gateway), convenções de código,
  modelo de dados, enums e decisões-chave de design.
- [fluxos.md](./fluxos.md) — fluxos de negócio passo a passo (cadastro
  com checagem de duplicidade, resolução de merge, fila de atendimento,
  registro de sessão, encaminhamento, acesso cross-unidade, indicadores,
  uso mobile offline-first).
- [lgpd.md](./lgpd.md) — base legal, criptografia em repouso, auditoria
  de acesso, minimização de dados no mobile, checklist de conformidade.
- [governanca-dados.md](./governanca-dados.md) — identidade canônica vs.
  registro de atendimento, ciclo de vida da deduplicação, RBAC,
  responsabilidade dos módulos de domínio sobre os dados.
- [SIGIS-MESTRE.md](./SIGIS-MESTRE.md) — documento mestre completo
  (a ser preenchido manualmente).

Fonte de verdade operacional do projeto: `claude.md` na raiz do
repositório — não repita o conteúdo dele aqui sem necessidade; os
documentos acima só reorganizam e cruzam referências do que já está lá.
