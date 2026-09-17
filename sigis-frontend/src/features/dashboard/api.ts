import { apiClient } from "@/lib/api/client";
import { ENDPOINTS } from "@/lib/api/endpoints";
import type {
  AlertaRecente,
  AtendimentosPorDiaItem,
  FilaPorServicoItem,
  Indicadores,
  PeriodoFiltro,
} from "@/lib/types/indicadores";

export async function fetchIndicadores(periodo: PeriodoFiltro): Promise<Indicadores> {
  const { data } = await apiClient.get<Indicadores>(ENDPOINTS.indicadores.base, { params: { periodo } });
  return data;
}

export async function fetchFilaPorServico(): Promise<FilaPorServicoItem[]> {
  const { data } = await apiClient.get<FilaPorServicoItem[]>(ENDPOINTS.indicadores.filaPorServico);
  return data;
}

export async function fetchAtendimentosPorDia(periodo: PeriodoFiltro): Promise<AtendimentosPorDiaItem[]> {
  const { data } = await apiClient.get<AtendimentosPorDiaItem[]>(ENDPOINTS.indicadores.atendimentosPorDia, {
    params: { periodo },
  });
  return data;
}

export async function fetchAlertasRecentes(): Promise<AlertaRecente[]> {
  const { data } = await apiClient.get<AlertaRecente[]>(ENDPOINTS.indicadores.alertasRecentes);
  return data;
}
