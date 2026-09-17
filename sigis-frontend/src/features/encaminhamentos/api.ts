import { apiClient } from "@/lib/api/client";
import { ENDPOINTS } from "@/lib/api/endpoints";
import { fetchPessoa } from "@/features/pacientes/api";
import { fetchUnidades } from "@/features/unidades/api";
import { mapPriorityFromApi, mapPriorityToApi, mapReferralStatusFromApi } from "@/lib/api/backendMappers";
import type {
  CriarEncaminhamentoRequest,
  Encaminhamento,
  RecusarEncaminhamentoRequest,
} from "@/lib/types/encaminhamento";

interface BackendReferral {
  id: string;
  personId: string;
  originUnitId: string;
  originUnitName: string;
  destinationUnitId: string;
  destinationUnitName: string;
  reason: string;
  priority: string;
  referralDate: string;
  status: string;
  correlationId: string;
}

/**
 * `ReferralResponse` real não devolve nome da pessoa, sigla das unidades
 * (só nome completo), nem dados de aceite/recusa/base-legal — completamos o
 * nome da pessoa e as siglas via `fetchPessoa`/`fetchUnidades`; os demais
 * campos ficam `undefined` (gap documentado).
 */
async function mapReferral(referral: BackendReferral): Promise<Encaminhamento> {
  const [pessoa, unidades] = await Promise.all([
    fetchPessoa(referral.personId).catch(() => undefined),
    fetchUnidades(),
  ]);

  const siglaPorId = new Map(unidades.map((u) => [u.id, u.sigla]));

  return {
    id: referral.id,
    personId: referral.personId,
    personName: pessoa?.fullName ?? referral.personId,
    unidadeOrigemId: referral.originUnitId,
    unidadeOrigemSigla: siglaPorId.get(referral.originUnitId) ?? referral.originUnitName,
    unidadeDestinoId: referral.destinationUnitId,
    unidadeDestinoSigla: siglaPorId.get(referral.destinationUnitId) ?? referral.destinationUnitName,
    motivo: referral.reason,
    prioridade: mapPriorityFromApi(referral.priority),
    dataEncaminhamento: referral.referralDate,
    status: mapReferralStatusFromApi(referral.status),
  };
}

export async function fetchEncaminhamento(id: string): Promise<Encaminhamento> {
  const { data } = await apiClient.get<BackendReferral>(ENDPOINTS.encaminhamentos.porId(id));
  return mapReferral(data);
}

export async function fetchEncaminhamentosPorPessoa(pessoaId: string): Promise<Encaminhamento[]> {
  const { data } = await apiClient.get<BackendReferral[]>(ENDPOINTS.encaminhamentos.porPessoa(pessoaId));
  return Promise.all(data.map(mapReferral));
}

export async function criarEncaminhamento(request: CriarEncaminhamentoRequest): Promise<Encaminhamento> {
  const { data } = await apiClient.post<BackendReferral>(ENDPOINTS.encaminhamentos.base, {
    personId: request.personId,
    originUnitId: request.unidadeOrigemId,
    destinationUnitId: request.unidadeDestinoId,
    reason: request.motivo,
    priority: mapPriorityToApi(request.prioridade),
  });
  return mapReferral(data);
}

/** T13 — encaminhamentos pendentes de decisao para a unidade do profissional logado. */
export async function fetchEncaminhamentosRecebidos(): Promise<Encaminhamento[]> {
  const { data } = await apiClient.get<BackendReferral[]>(ENDPOINTS.encaminhamentos.recebidos);
  return Promise.all(data.map(mapReferral));
}

/** Encaminhamentos enviados pela unidade do profissional logado. */
export async function fetchEncaminhamentosEnviados(): Promise<Encaminhamento[]> {
  const { data } = await apiClient.get<BackendReferral[]>(ENDPOINTS.encaminhamentos.enviados);
  return Promise.all(data.map(mapReferral));
}

export async function aceitarEncaminhamento(id: string): Promise<Encaminhamento> {
  const { data } = await apiClient.post<BackendReferral>(ENDPOINTS.encaminhamentos.aceitar(id));
  return mapReferral(data);
}

export async function recusarEncaminhamento(id: string, request: RecusarEncaminhamentoRequest): Promise<Encaminhamento> {
  const { data } = await apiClient.post<BackendReferral>(ENDPOINTS.encaminhamentos.recusar(id), {
    motivo: request.motivo,
  });
  return mapReferral(data);
}
