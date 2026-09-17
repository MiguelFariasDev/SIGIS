import { apiClient } from "@/lib/api/client";
import { ENDPOINTS } from "@/lib/api/endpoints";
import type { StatusFila } from "@/lib/types/enums";
import type { FilaAtendimento } from "@/lib/types/fila";

export async function fetchFilaPorUnidade(unidadeId: string, especialidade?: string): Promise<FilaAtendimento[]> {
  const { data } = await apiClient.get<FilaAtendimento[]>(ENDPOINTS.filas.porUnidade(unidadeId), {
    params: { especialidade: especialidade || undefined },
  });
  return data;
}

/** Visao "rede inteira" (T07 nivel 1) — usada pelo COORDENADOR. */
export async function fetchFilaRede(filtros: { unidadeId?: string; especialidade?: string } = {}): Promise<FilaAtendimento[]> {
  const { data } = await apiClient.get<FilaAtendimento[]>(ENDPOINTS.filas.base, {
    params: { unidadeId: filtros.unidadeId || undefined, especialidade: filtros.especialidade || undefined },
  });
  return data;
}

export async function fetchFilaPorPessoa(pessoaId: string): Promise<FilaAtendimento[]> {
  const { data } = await apiClient.get<FilaAtendimento[]>(ENDPOINTS.filas.base, { params: { personId: pessoaId } });
  return data;
}

export async function atualizarStatusFila(id: string, status: StatusFila): Promise<FilaAtendimento> {
  const { data } = await apiClient.patch<FilaAtendimento>(ENDPOINTS.filas.porId(id), { status });
  return data;
}

export async function chamarProximo(unidadeId: string): Promise<FilaAtendimento> {
  const { data } = await apiClient.post<FilaAtendimento>(ENDPOINTS.filas.chamarProximo(unidadeId));
  return data;
}
