import { apiClient } from "@/lib/api/client";
import { ENDPOINTS } from "@/lib/api/endpoints";
import type {
  CriarEncaminhamentoRequest,
  Encaminhamento,
  RecusarEncaminhamentoRequest,
} from "@/lib/types/encaminhamento";

export async function fetchEncaminhamento(id: string): Promise<Encaminhamento> {
  const { data } = await apiClient.get<Encaminhamento>(ENDPOINTS.encaminhamentos.porId(id));
  return data;
}

export async function fetchEncaminhamentosPorPessoa(pessoaId: string): Promise<Encaminhamento[]> {
  const { data } = await apiClient.get<Encaminhamento[]>(ENDPOINTS.encaminhamentos.porPessoa(pessoaId));
  return data;
}

export async function criarEncaminhamento(request: CriarEncaminhamentoRequest): Promise<Encaminhamento> {
  const { data } = await apiClient.post<Encaminhamento>(ENDPOINTS.encaminhamentos.base, request);
  return data;
}

/** T13 — encaminhamentos pendentes de decisao para a unidade do profissional logado. */
export async function fetchEncaminhamentosRecebidos(unidadeDestinoId: string): Promise<Encaminhamento[]> {
  const { data } = await apiClient.get<Encaminhamento[]>(ENDPOINTS.encaminhamentos.recebidos, {
    params: { unidadeDestinoId },
  });
  return data;
}

/** Encaminhamentos enviados pela unidade do profissional logado. */
export async function fetchEncaminhamentosEnviados(unidadeOrigemId: string): Promise<Encaminhamento[]> {
  const { data } = await apiClient.get<Encaminhamento[]>(ENDPOINTS.encaminhamentos.enviados, {
    params: { unidadeOrigemId },
  });
  return data;
}

export async function aceitarEncaminhamento(id: string): Promise<Encaminhamento> {
  const { data } = await apiClient.post<Encaminhamento>(ENDPOINTS.encaminhamentos.aceitar(id));
  return data;
}

export async function recusarEncaminhamento(id: string, request: RecusarEncaminhamentoRequest): Promise<Encaminhamento> {
  const { data } = await apiClient.post<Encaminhamento>(ENDPOINTS.encaminhamentos.recusar(id), request);
  return data;
}
