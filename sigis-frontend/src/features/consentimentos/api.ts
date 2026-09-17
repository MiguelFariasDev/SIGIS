import { apiClient } from "@/lib/api/client";
import { ENDPOINTS } from "@/lib/api/endpoints";
import type { ConsentimentosFiltros, PersonConsentComPessoa } from "@/lib/types/personConsent";

interface BackendPersonConsentWithPerson {
  id: string;
  personId: string;
  personName: string;
  type: string;
  granted: boolean;
  grantedAt: string;
  grantedByGuardianId?: string;
  revokedAt?: string;
  version: string;
  evidence?: string;
  createdAt: string;
}

/** T17 — painel global de consentimentos (`GET /api/consentimentos`, DPO/coordenador). */
export async function fetchConsentimentosGlobal(filtros: ConsentimentosFiltros): Promise<PersonConsentComPessoa[]> {
  const { data } = await apiClient.get<BackendPersonConsentWithPerson[]>(ENDPOINTS.consentimentos.base, {
    params: {
      personId: filtros.personId || undefined,
      type: filtros.type || undefined,
      status: filtros.status || undefined,
      dataInicio: filtros.dataInicio || undefined,
      dataFim: filtros.dataFim || undefined,
    },
  });

  return data.map((c) => ({
    id: c.id,
    personId: c.personId,
    personName: c.personName,
    type: c.type as PersonConsentComPessoa["type"],
    granted: c.granted,
    grantedAt: c.grantedAt,
    grantedByGuardianId: c.grantedByGuardianId,
    revokedAt: c.revokedAt,
    version: c.version,
    evidence: c.evidence,
    createdAt: c.createdAt,
  }));
}
