import { apiClient } from "@/lib/api/client";
import { ENDPOINTS } from "@/lib/api/endpoints";
import { mapSecretariatFromApi } from "@/lib/api/backendMappers";
import type { UnidadeServico } from "@/lib/types/unidade";

interface BackendServiceUnit {
  id: string;
  name: string;
  acronym: string;
  secretariat: string;
}

/** Endpoint adicionado nesta integração (`GET /api/unidades`) — não existia entre os 16 controllers originais. */
export async function fetchUnidades(): Promise<UnidadeServico[]> {
  const { data } = await apiClient.get<BackendServiceUnit[]>(ENDPOINTS.unidades.base);
  return data.map((u) => ({
    id: u.id,
    nome: u.name,
    sigla: u.acronym,
    secretariaResponsavel: mapSecretariatFromApi(u.secretariat),
  }));
}
