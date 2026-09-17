export const ENDPOINTS = {
  auth: {
    login: "/api/auth/login",
  },
  pessoas: {
    base: "/api/pessoas",
    porId: (id: string) => `/api/pessoas/${id}`,
    /** Rota real é "busca" (não "buscar"); parâmetros aceitos: termo, limit. */
    buscar: "/api/pessoas/busca",
    linhaDoTempo: (id: string) => `/api/pessoas/${id}/linha-do-tempo`,
    mesclar: "/api/pessoas/mesclar",
    solicitarAcesso: (id: string) => `/api/pessoas/${id}/solicitar-acesso`,
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
  },
  /** Endpoint mínimo adicionado nesta integração — ver UnitsController (backend). */
  unidades: {
    base: "/api/unidades",
    fila: (unidadeId: string) => `/api/unidades/${unidadeId}/fila`,
  },
  filas: {
    /** Chama uma entrada de fila específica (não "o próximo da unidade" — a UI escolhe a entrada antes de chamar). */
    chamar: (queueEntryId: string) => `/api/filas/${queueEntryId}/chamar`,
    comparecimento: (queueEntryId: string) => `/api/filas/${queueEntryId}/comparecimento`,
  },
  atendimentos: {
    base: "/api/atendimentos",
    porId: (id: string) => `/api/atendimentos/${id}`,
    porPessoa: (pessoaId: string) => `/api/atendimentos/pessoa/${pessoaId}`,
    comparecimento: (id: string) => `/api/atendimentos/${id}/comparecimento`,
  },
  encaminhamentos: {
    base: "/api/encaminhamentos",
    recebidos: "/api/encaminhamentos/recebidos",
    /** Não existe GET simples por id no backend — "rastreio" já é o get-by-id (mesmo DTO). */
    porId: (id: string) => `/api/encaminhamentos/${id}/rastreio`,
    porPessoa: (pessoaId: string) => `/api/encaminhamentos/pessoa/${pessoaId}`,
    enviados: "/api/encaminhamentos/enviados",
    aceitar: (id: string) => `/api/encaminhamentos/${id}/aceitar`,
    recusar: (id: string) => `/api/encaminhamentos/${id}/recusar`,
    rastreio: (id: string) => `/api/encaminhamentos/${id}/rastreio`,
  },
  duplicidades: {
    pendentes: "/api/duplicidades/pendentes",
    porId: (id: string) => `/api/duplicidades/${id}`,
    resolver: (id: string) => `/api/duplicidades/${id}/resolver`,
    falsoPositivo: (id: string) => `/api/duplicidades/${id}/falso-positivo`,
  },
  indicadores: {
    painel: "/api/indicadores/painel",
    filaPorServico: "/api/indicadores/fila-por-servico",
    atendimentosPorDia: "/api/indicadores/atendimentos-por-dia",
    alertasRecentes: "/api/indicadores/alertas-recentes",
  },
  auditoria: {
    acessos: "/api/auditoria/acessos",
    acessosCross: "/api/auditoria/acessos-cross",
    exportarCsv: "/api/auditoria/export-csv",
  },
  consentimentos: {
    base: "/api/consentimentos",
  },
  legalBasis: {
    base: "/api/legal-basis",
  },
} as const;
