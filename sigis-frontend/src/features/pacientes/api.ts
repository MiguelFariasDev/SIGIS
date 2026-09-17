import { apiClient } from "@/lib/api/client";
import { ENDPOINTS } from "@/lib/api/endpoints";
import type { PersonClinicalProfile, UpsertClinicalProfileRequest } from "@/lib/types/personClinicalProfile";
import type { FamilyComposition, UpsertFamilyCompositionRequest } from "@/lib/types/familyComposition";
import type { DevelopmentMilestones, UpsertDevelopmentMilestonesRequest } from "@/lib/types/developmentMilestones";
import type { GrantConsentRequest, PersonConsent } from "@/lib/types/personConsent";
import type { CreatePersonRequest, Person, PersonSearchFilters, PersonSearchResult } from "@/lib/types/person";
import type { TimelineResponse } from "@/lib/types/timeline";
import type { ConcurrentTreatment, CreateConcurrentTreatmentRequest } from "@/lib/types/concurrentTreatment";
import type { CreateSchoolHistoryRequest, SchoolHistory } from "@/lib/types/schoolHistory";
import type { CreateLearningDifficultyRequest, LearningDifficulties } from "@/lib/types/learningDifficulties";

export async function buscarPessoas(filtros: PersonSearchFilters): Promise<PersonSearchResult[]> {
  const { data } = await apiClient.get<PersonSearchResult[]>(ENDPOINTS.pessoas.buscar, {
    params: {
      termo: filtros.term || undefined,
      servico: filtros.service || undefined,
      status: filtros.status || undefined,
      prioridade: filtros.priority || undefined,
    },
  });
  return data;
}

export async function fetchPessoa(id: string): Promise<Person> {
  const { data } = await apiClient.get<Person>(ENDPOINTS.pessoas.porId(id));
  return data;
}

/**
 * Busca a linha do tempo unificada da pessoa. `secretariasLiberadas`
 * simula o resultado de solicitacoes de acesso ja aprovadas nesta
 * sessao (RF12) — eventos daquelas secretarias deixam de vir com
 * `locked: true`.
 */
export async function fetchTimeline(id: string, secretariasLiberadas: string[] = []): Promise<TimelineResponse> {
  const { data } = await apiClient.get<TimelineResponse>(ENDPOINTS.pessoas.linhaDoTempo(id), {
    params: secretariasLiberadas.length > 0 ? { nivel: secretariasLiberadas.join(",") } : undefined,
  });
  return data;
}

export async function criarPessoa(request: CreatePersonRequest): Promise<Person> {
  const { data } = await apiClient.post<Person>(ENDPOINTS.pessoas.base, request);
  return data;
}

export async function fetchPerfilClinico(personId: string): Promise<PersonClinicalProfile | null> {
  const { data } = await apiClient.get<PersonClinicalProfile | null>(ENDPOINTS.pessoas.perfilClinico(personId));
  return data;
}

export async function salvarPerfilClinico(personId: string, request: UpsertClinicalProfileRequest) {
  const { data } = await apiClient.put(ENDPOINTS.pessoas.perfilClinico(personId), request);
  return data;
}

export async function fetchTratamentosConcomitantes(personId: string): Promise<ConcurrentTreatment[]> {
  const { data } = await apiClient.get<ConcurrentTreatment[]>(ENDPOINTS.pessoas.tratamentosConcomitantes(personId));
  return data;
}

export async function criarTratamentoConcomitante(personId: string, request: CreateConcurrentTreatmentRequest) {
  const { data } = await apiClient.post(ENDPOINTS.pessoas.tratamentosConcomitantes(personId), request);
  return data;
}

export async function excluirTratamentoConcomitante(personId: string, treatmentId: string) {
  await apiClient.delete(ENDPOINTS.pessoas.tratamentoConcomitante(personId, treatmentId));
}

export async function fetchHistoricoEscolar(personId: string): Promise<SchoolHistory[]> {
  const { data } = await apiClient.get<SchoolHistory[]>(ENDPOINTS.pessoas.historicoEscolar(personId));
  return data;
}

export async function criarHistoricoEscolar(personId: string, request: CreateSchoolHistoryRequest) {
  const { data } = await apiClient.post(ENDPOINTS.pessoas.historicoEscolar(personId), request);
  return data;
}

export async function fetchComposicaoFamiliar(personId: string): Promise<FamilyComposition | null> {
  const { data } = await apiClient.get<FamilyComposition | null>(ENDPOINTS.pessoas.composicaoFamiliar(personId));
  return data;
}

export async function salvarComposicaoFamiliar(personId: string, request: UpsertFamilyCompositionRequest) {
  const { data } = await apiClient.put(ENDPOINTS.pessoas.composicaoFamiliar(personId), request);
  return data;
}

export async function fetchDesenvolvimento(personId: string): Promise<DevelopmentMilestones | null> {
  const { data } = await apiClient.get<DevelopmentMilestones | null>(ENDPOINTS.pessoas.desenvolvimento(personId));
  return data;
}

export async function salvarDesenvolvimento(personId: string, request: UpsertDevelopmentMilestonesRequest) {
  const { data } = await apiClient.put(ENDPOINTS.pessoas.desenvolvimento(personId), request);
  return data;
}

export async function fetchDificuldadesAprendizagem(personId: string): Promise<LearningDifficulties[]> {
  const { data } = await apiClient.get<LearningDifficulties[]>(ENDPOINTS.pessoas.dificuldadesAprendizagem(personId));
  return data;
}

export async function criarDificuldadeAprendizagem(personId: string, request: CreateLearningDifficultyRequest) {
  const { data } = await apiClient.post(ENDPOINTS.pessoas.dificuldadesAprendizagem(personId), request);
  return data;
}

export async function fetchConsentimentos(personId: string): Promise<PersonConsent[]> {
  const { data } = await apiClient.get<PersonConsent[]>(ENDPOINTS.pessoas.consentimentos(personId));
  return data;
}

export async function registrarConsentimento(personId: string, request: GrantConsentRequest) {
  const { data } = await apiClient.post(ENDPOINTS.pessoas.consentimentos(personId), request);
  return data;
}

export async function revogarConsentimento(personId: string, consentId: string) {
  const { data } = await apiClient.delete(ENDPOINTS.pessoas.consentimento(personId, consentId));
  return data;
}

export interface SolicitarAcessoRequest {
  legalBasisId: string;
  purpose: string;
  justification: string;
}

export async function solicitarAcesso(personId: string, request: SolicitarAcessoRequest) {
  const { data } = await apiClient.post(ENDPOINTS.pessoas.solicitarAcesso(personId), request);
  return data;
}
