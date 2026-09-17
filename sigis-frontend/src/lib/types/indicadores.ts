export type PeriodoFiltro = "HOJE" | "7D" | "30D";

export interface Indicadores {
  atendimentosNoPeriodo: number;
  faltasNoPeriodo: number;
  encaminhamentosAtivos: number;
  duplicidadesPendentes: number;
}

export interface FilaPorServicoItem {
  servico: string;
  aguardando: number;
  emAtendimento: number;
}

export interface AtendimentosPorDiaItem {
  data: string; // ISO date
  quantidade: number;
}

export interface AlertaRecente {
  id: string;
  tipo: "DUPLICIDADE" | "BUSCA_ATIVA";
  descricao: string;
  dataHora: string;
}
