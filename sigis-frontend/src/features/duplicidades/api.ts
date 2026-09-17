import { apiClient } from "@/lib/api/client";
import { ENDPOINTS } from "@/lib/api/endpoints";
import { fetchPessoa } from "@/features/pacientes/api";
import type { AlertaDuplicidade, ResolverDuplicidadeRequest } from "@/lib/types/duplicidade";
import { StatusAlertaDuplicidade } from "@/lib/types/enums";

interface BackendDuplicateAlert {
  id: string;
  personId1: string;
  personName1: string;
  personId2: string;
  personName2: string;
  similarityScore: number;
  createdAt: string;
}

/**
 * O backend real não devolve `motivoMatch`/`status`/dados de resolução no
 * `DuplicateAlertResponse` (só id, os dois pares id+nome, score e data de
 * criação) — completamos com os cadastros completos (para a comparação
 * lado-a-lado) e um texto fixo de motivo (gap documentado).
 */
async function mapDuplicateAlert(alert: BackendDuplicateAlert): Promise<AlertaDuplicidade> {
  const [person1, person2] = await Promise.all([
    fetchPessoa(alert.personId1).catch(() => undefined),
    fetchPessoa(alert.personId2).catch(() => undefined),
  ]);

  return {
    id: alert.id,
    personId1: alert.personId1,
    personId2: alert.personId2,
    person1,
    person2,
    scoreSimilaridade: alert.similarityScore,
    motivoMatch: "Nome e data de nascimento semelhantes",
    status: StatusAlertaDuplicidade.PENDENTE,
    criadoEm: alert.createdAt,
  };
}

/**
 * `status` é aceito por compatibilidade com os chamadores existentes, mas o
 * backend real só expõe a fila de pendentes (`GET /api/duplicidades/pendentes`)
 * — não há filtro por outros status (gap documentado).
 */
export async function fetchDuplicidades(_status?: StatusAlertaDuplicidade): Promise<AlertaDuplicidade[]> {
  const { data } = await apiClient.get<BackendDuplicateAlert[]>(ENDPOINTS.duplicidades.pendentes);
  return Promise.all(data.map(mapDuplicateAlert));
}

export async function fetchDuplicidade(id: string): Promise<AlertaDuplicidade> {
  const { data } = await apiClient.get<BackendDuplicateAlert>(ENDPOINTS.duplicidades.porId(id));
  return mapDuplicateAlert(data);
}

export async function resolverDuplicidade(
  id: string,
  request: ResolverDuplicidadeRequest,
): Promise<AlertaDuplicidade> {
  const { data } = await apiClient.post<BackendDuplicateAlert>(ENDPOINTS.duplicidades.resolver(id), request);
  return mapDuplicateAlert(data);
}
