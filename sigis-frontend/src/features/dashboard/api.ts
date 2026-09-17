import { apiClient } from "@/lib/api/client";
import { ENDPOINTS } from "@/lib/api/endpoints";
import type {
  AlertaRecente,
  AtendimentosPorDiaItem,
  FilaPorServicoItem,
  Indicadores,
  PeriodoFiltro,
} from "@/lib/types/indicadores";

interface BackendIndicatorsPanel {
  totalNaFilaAtiva: number;
  filaPorStatus: Record<string, number>;
  filaPorPrioridade: Record<string, number>;
  tempoMedioEsperaMinutos: number | null;
  totalAtendimentos: number;
  atendimentosPorStatus: Record<string, number>;
  totalEncaminhamentos: number;
  encaminhamentosPorStatus: Record<string, number>;
}

/**
 * `GET /api/indicadores/painel` real devolve um painel agregado bem
 * diferente do resumo fixo esperado pela UI (`Indicadores`) — derivamos os 4
 * cartoes a partir dele. `duplicidadesPendentes` não faz parte do painel;
 * completamos com uma segunda chamada leve à fila de duplicidades pendentes.
 */
export async function fetchIndicadores(_periodo: PeriodoFiltro): Promise<Indicadores> {
  const [{ data: painel }, { data: duplicidades }] = await Promise.all([
    apiClient.get<BackendIndicatorsPanel>(ENDPOINTS.indicadores.painel),
    apiClient.get<unknown[]>(ENDPOINTS.duplicidades.pendentes),
  ]);

  return {
    atendimentosNoPeriodo: painel.totalAtendimentos,
    faltasNoPeriodo: painel.atendimentosPorStatus.Absent ?? 0,
    encaminhamentosAtivos: (painel.encaminhamentosPorStatus.Pending ?? 0) + (painel.encaminhamentosPorStatus.Accepted ?? 0),
    duplicidadesPendentes: duplicidades.length,
  };
}

interface BackendFilaPorServico {
  servico: string;
  aguardando: number;
  emAtendimento: number;
}

export async function fetchFilaPorServico(): Promise<FilaPorServicoItem[]> {
  const { data } = await apiClient.get<BackendFilaPorServico[]>(ENDPOINTS.indicadores.filaPorServico);
  return data;
}

interface BackendAtendimentosPorDia {
  data: string;
  quantidade: number;
}

export async function fetchAtendimentosPorDia(periodo: PeriodoFiltro): Promise<AtendimentosPorDiaItem[]> {
  const { data } = await apiClient.get<BackendAtendimentosPorDia[]>(ENDPOINTS.indicadores.atendimentosPorDia, {
    params: { periodo },
  });
  return data.map((item) => ({ data: item.data, quantidade: item.quantidade }));
}

interface BackendAlertaRecente {
  id: string;
  tipo: "DUPLICIDADE" | "BUSCA_ATIVA";
  descricao: string;
  dataHora: string;
}

export async function fetchAlertasRecentes(): Promise<AlertaRecente[]> {
  const { data } = await apiClient.get<BackendAlertaRecente[]>(ENDPOINTS.indicadores.alertasRecentes);
  return data;
}
