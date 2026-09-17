import { apiClient } from "@/lib/api/client";
import { ENDPOINTS } from "@/lib/api/endpoints";
import type { AlertaDuplicidade, ResolverDuplicidadeRequest } from "@/lib/types/duplicidade";
import type { StatusAlertaDuplicidade } from "@/lib/types/enums";

export async function fetchDuplicidades(status?: StatusAlertaDuplicidade): Promise<AlertaDuplicidade[]> {
  const { data } = await apiClient.get<AlertaDuplicidade[]>(ENDPOINTS.duplicidades.base, {
    params: { status },
  });
  return data;
}

export async function fetchDuplicidade(id: string): Promise<AlertaDuplicidade> {
  const { data } = await apiClient.get<AlertaDuplicidade>(ENDPOINTS.duplicidades.porId(id));
  return data;
}

export async function resolverDuplicidade(
  id: string,
  request: ResolverDuplicidadeRequest,
): Promise<AlertaDuplicidade> {
  const { data } = await apiClient.post<AlertaDuplicidade>(ENDPOINTS.duplicidades.resolver(id), request);
  return data;
}
