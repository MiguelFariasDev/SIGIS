export const ENDPOINTS = {
  auth: {
    login: "/api/auth/login",
  },
  pessoas: {
    base: "/api/pessoas",
    porId: (id: string) => `/api/pessoas/${id}`,
    buscar: "/api/pessoas/buscar",
    linhaDoTempo: (id: string) => `/api/pessoas/${id}/linha-do-tempo`,
    perfilClinico: (id: string) => `/api/pessoas/${id}/perfil-clinico`,
    tratamentosConcomitantes: (id: string) => `/api/pessoas/${id}/tratamentos-concomitantes`,
    tratamentoConcomitante: (id: string, treatmentId: string) =>
      `/api/pessoas/${id}/tratamentos-concomitantes/${treatmentId}`,
    historicoEscolar: (id: string) => `/api/pessoas/${id}/historico-escolar`,
    composicaoFamiliar: (id: string) => `/api/pessoas/${id}/composicao-familiar`,
    desenvolvimento: (id: string) => `/api/pessoas/${id}/desenvolvimento`,
    dificuldadesAprendizagem: (id: string) => `/api/pessoas/${id}/dificuldades-aprendizagem`,
    consentimentos: (id: string) => `/api/pessoas/${id}/consentimentos`,
    consentimento: (id: string, consentId: string) => `/api/pessoas/${id}/consentimentos/${consentId}`,
    solicitarAcesso: (id: string) => `/api/pessoas/${id}/solicitar-acesso`,
  },
  unidades: {
    base: "/api/unidades",
  },
  profissionais: {
    base: "/api/profissionais",
  },
  filas: {
    base: "/api/filas",
    porUnidade: (unidadeId: string) => `/api/filas/unidade/${unidadeId}`,
    porId: (id: string) => `/api/filas/${id}`,
    chamarProximo: (unidadeId: string) => `/api/filas/unidade/${unidadeId}/chamar-proximo`,
  },
  atendimentos: {
    base: "/api/atendimentos",
    porId: (id: string) => `/api/atendimentos/${id}`,
    porPessoa: (pessoaId: string) => `/api/atendimentos/pessoa/${pessoaId}`,
  },
  encaminhamentos: {
    base: "/api/encaminhamentos",
    porId: (id: string) => `/api/encaminhamentos/${id}`,
    porPessoa: (pessoaId: string) => `/api/encaminhamentos/pessoa/${pessoaId}`,
    recebidos: "/api/encaminhamentos/recebidos",
    enviados: "/api/encaminhamentos/enviados",
    aceitar: (id: string) => `/api/encaminhamentos/${id}/aceitar`,
    recusar: (id: string) => `/api/encaminhamentos/${id}/recusar`,
  },
  duplicidades: {
    base: "/api/duplicidades",
    porId: (id: string) => `/api/duplicidades/${id}`,
    resolver: (id: string) => `/api/duplicidades/${id}/resolver`,
  },
  indicadores: {
    base: "/api/indicadores",
    filaPorServico: "/api/indicadores/fila-por-servico",
    atendimentosPorDia: "/api/indicadores/atendimentos-por-dia",
    alertasRecentes: "/api/indicadores/alertas-recentes",
  },
  auditoria: {
    base: "/api/auditoria",
    exportarCsv: "/api/auditoria/exportar",
  },
  legalBasis: {
    base: "/api/legal-basis",
  },
  consentimentos: {
    /** Listagem global (T17 — painel DPO/COORDENADOR), distinta de `pessoas.consentimentos`. */
    base: "/api/consentimentos",
  },
} as const;
