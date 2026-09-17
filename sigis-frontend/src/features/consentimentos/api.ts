import { apiClient } from "@/lib/api/client";
import { ENDPOINTS } from "@/lib/api/endpoints";
import type { ConsentimentosFiltros, PersonConsentComPessoa } from "@/lib/types/personConsent";

export async function fetchConsentimentosGlobal(filtros: ConsentimentosFiltros): Promise<PersonConsentComPessoa[]> {
  const { data } = await apiClient.get<PersonConsentComPessoa[]>(ENDPOINTS.consentimentos.base, {
    params: {
      personId: filtros.personId || undefined,
      type: filtros.type || undefined,
      status: filtros.status || undefined,
      dataInicio: filtros.dataInicio || undefined,
      dataFim: filtros.dataFim || undefined,
    },
  });
  return data;
}
