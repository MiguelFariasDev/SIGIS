# SIGIS — LGPD e proteção de dados

> Fonte: `claude.md` (seções 9.5, 10.5, 11 e a decisão de design "LGPD by
> design"). O SIGIS trata dado de saúde de pessoas com TEA — dado sensível
> nos termos do art. 5º, II da Lei 13.709/2018 (LGPD). Este documento
> consolida os pontos obrigatórios de conformidade que já têm decisão
> tomada no `claude.md`. A seção 16 do `claude.md` ("LGPD — pontos
> obrigatórios", versão detalhada) ainda não foi preenchida no arquivo
> fonte no momento da geração deste documento; atualizar aqui quando
> disponível.

## 1. Por que isso importa para o SIGIS

- Dado de saúde é **dado sensível** (art. 5º, II, LGPD).
- Dado de criança/adolescente com TEA acumula duas camadas de proteção:
  dado sensível de saúde **e** dado de titular vulnerável.
- O sistema centraliza informações hoje fragmentadas em cadernos físicos
  entre 5 serviços de 3 secretarias diferentes (Saúde, Educação,
  Assistência Social) — isso aumenta a superfície de risco se o acesso
  não for controlado e auditado desde o desenho do sistema.

## 2. Base legal (RNF02)

- Base legal aplicável: **art. 11, II, "a" e "f"** da LGPD — tutela da
  saúde, em procedimento realizado por serviço de saúde ou de assistência
  social.
- O campo `base_legal` em `LOG_ACESSO` é **obrigatório e gravado como
  dado real**, não como checkbox decorativo de interface. Cada acesso
  cross-unidade registrado precisa indicar sob qual base legal o acesso
  ocorreu.

## 3. Criptografia em repouso

- CPF e CNS em `PESSOA` são armazenados **criptografados** via extensão
  `pgcrypto` do PostgreSQL (ver
  [system-design.md](./system-design.md), seção "Extensões PostgreSQL").
- Nenhum outro campo do modelo de dados atual foi definido como
  obrigatoriamente criptografado no `claude.md` — se novos campos
  sensíveis forem adicionados (ex.: diagnóstico textual livre em
  `dados_formulario`), reavaliar a necessidade de criptografia em
  repouso ou de mascaramento antes de expor via API.

## 4. Auditoria de acesso (RNF03, RF13)

- **Nenhum dado sensível é lido cross-unidade sem deixar rastro.**
- Todo acesso de um profissional a dados de paciente de **unidade
  diferente da originadora** gera um registro em `LOG_ACESSO`:
  `pessoa_id`, `profissional_id`, `acao`, `base_legal`, `data_hora`.
- O acesso à timeline consolidada (RF08) — que por definição cruza
  unidades — exige justificativa/consentimento registrado além do log
  de acesso (ver RF12 e [fluxos.md](./fluxos.md), seção 6).
- O papel `AUDITOR` (RBAC, ver
  [governanca-dados.md](./governanca-dados.md)) tem acesso **somente
  leitura** a `LOG_ACESSO`, sem acesso aos dados clínicos em si — essa é
  a persona "Auditor/DPO municipal" descrita em
  [requisitos.md](./requisitos.md).

## 5. Minimização de dados no mobile (RNF06)

- O app Flutter (uso em campo, offline-first) **não cacheia o histórico
  completo** do paciente localmente.
- O cache local (`sqflite`) deve conter apenas o mínimo necessário para
  as operações offline previstas em RF14: busca de paciente,
  confirmação de comparecimento/falta, registro de sessão rápida.
- Ao implementar a camada de sincronização, tratar a fila local como
  dado sensível também: `shared_preferences` não é apropriado para
  guardar CNS/CPF em claro; se necessário, usar armazenamento
  criptografado no dispositivo.

## 6. RBAC como controle de acesso a dado sensível (RF12)

Ver [governanca-dados.md](./governanca-dados.md) para o detalhamento dos
3 papéis (`PROFISSIONAL`, `COORDENADOR`, `AUDITOR`). Do ponto de vista de
LGPD, o RBAC é o mecanismo técnico que implementa o princípio de
**minimização** e **necessidade** (art. 6º, III): por padrão, cada
profissional só acessa dados da própria unidade.

## 7. Deduplicação e dado sensível

A deduplicação probabilística (`pg_trgm` sobre nome + data de
nascimento) processa dado pessoal para fins de identificação — isso é
compatível com a base legal do art. 11, mas reforça por que a fusão de
cadastros **nunca é automática**: uma fusão errada pode misturar o
histórico de saúde de duas pessoas diferentes. Toda fusão passa por
`ALERTA_DUPLICIDADE` com revisão humana obrigatória (ver
[fluxos.md](./fluxos.md), seção 2).

## 8. Checklist de conformidade (estado atual do design)

- [x] Base legal explícita e gravada (`LOG_ACESSO.base_legal`)
- [x] Criptografia em repouso para CPF/CNS (pgcrypto)
- [x] Trilha de auditoria de leitura/escrita cross-unidade
- [x] Minimização de dados no mobile (sem histórico completo em cache)
- [x] Controle de acesso por papel (RBAC de 3 níveis)
- [x] Fusão de cadastros nunca automática (revisão humana obrigatória)
- [ ] Política de retenção/expurgo de dados — **não definida** no
      `claude.md` disponível; definir antes de produção real.
- [ ] Processo de atendimento a direitos do titular (acesso, correção,
      eliminação — arts. 17–22 LGPD) — **não definido** no `claude.md`
      disponível; o titular aqui costuma ser o responsável legal da
      pessoa com TEA quando menor de idade.
- [ ] Relatório de Impacto à Proteção de Dados (RIPD/DPIA) — recomendado
      dado o volume de dado sensível de saúde infantil; não mencionado
      no `claude.md` disponível.

## 9. O que ainda falta detalhar

A seção 16 do `claude.md` ("LGPD — pontos obrigatórios") consta no
índice mas seu conteúdo não estava presente no arquivo fonte no momento
da geração deste documento. Os itens acima cobrem o que já está decidido
nas seções 9.5, 10.5 e 11. Atualizar este documento (e o checklist da
seção 8) assim que a versão completa estiver disponível em
[SIGIS-MESTRE.md](./SIGIS-MESTRE.md).
