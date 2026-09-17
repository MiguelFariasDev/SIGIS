import { apiClient } from "@/lib/api/client";
import { ENDPOINTS } from "@/lib/api/endpoints";
import { fetchUnidades } from "@/features/unidades/api";
import { mapSecretariatToApi } from "@/lib/api/backendMappers";
import { NotFoundError } from "@/lib/api/errors";
import type { PersonClinicalProfile, UpsertClinicalProfileRequest } from "@/lib/types/personClinicalProfile";
import type { FamilyComposition, UpsertFamilyCompositionRequest } from "@/lib/types/familyComposition";
import type { DevelopmentMilestones, UpsertDevelopmentMilestonesRequest } from "@/lib/types/developmentMilestones";
import type { GrantConsentRequest, PersonConsent } from "@/lib/types/personConsent";
import type {
  CreatePersonRequest,
  DuplicateFoundResponse,
  Person,
  PersonSearchFilters,
  PersonSearchResult,
} from "@/lib/types/person";
import type { TimelineEvento, TimelineEventoTipo, TimelineResponse } from "@/lib/types/timeline";
import type { ConcurrentTreatment, CreateConcurrentTreatmentRequest } from "@/lib/types/concurrentTreatment";
import type { CreateSchoolHistoryRequest, SchoolHistory } from "@/lib/types/schoolHistory";
import type { CreateLearningDifficultyRequest, LearningDifficulties } from "@/lib/types/learningDifficulties";

interface BackendPersonSummary {
  id: string;
  name: string;
  birthDate: string;
  motherName?: string;
}

/** Backend real: `GET /api/pessoas/busca` só aceita `termo`/`limit` — servico/status/prioridade não existem no filtro (gap documentado). */
export async function buscarPessoas(filtros: PersonSearchFilters): Promise<PersonSearchResult[]> {
  const { data } = await apiClient.get<BackendPersonSummary[]>(ENDPOINTS.pessoas.buscar, {
    params: { termo: filtros.term || undefined, limit: 50 },
  });
  return data.map((p) => ({ id: p.id, fullName: p.name, birthDate: p.birthDate, motherName: p.motherName }));
}

interface BackendPersonDetail {
  id: string;
  name: string;
  birthDate: string;
  cns?: string;
  cpf?: string;
  motherName?: string;
  gender?: string;
  raceColor?: string;
  phone?: string;
  email?: string;
  address?: string;
  naturality?: string;
  currentSchool?: string;
  grade?: string;
  shift?: string;
  classGroup?: string;
  zone?: string;
  schoolEnrollment?: string;
  referredBySchool?: boolean;
  disabilityTypes?: string;
  needsSpecialEducation?: boolean;
  attendsTutoring?: boolean;
  hasFailedGrade?: boolean;
  createdAt: string;
  updatedAt: string;
}

function mapPersonFromApi(p: BackendPersonDetail): Person {
  return {
    id: p.id,
    fullName: p.name,
    birthDate: p.birthDate,
    cns: p.cns,
    cpf: p.cpf,
    motherName: p.motherName,
    gender: p.gender,
    raceColor: p.raceColor,
    phone: p.phone,
    email: p.email,
    address: p.address,
    naturality: p.naturality,
    currentSchool: p.currentSchool,
    grade: p.grade,
    shift: p.shift,
    classGroup: p.classGroup,
    zone: p.zone as Person["zone"],
    schoolEnrollment: p.schoolEnrollment,
    referredBySchool: p.referredBySchool,
    needsSpecialEducation: p.needsSpecialEducation,
    attendsTutoring: p.attendsTutoring,
    hasFailedGrade: p.hasFailedGrade,
    disabilityTypes: p.disabilityTypes,
    createdAt: p.createdAt,
    updatedAt: p.updatedAt,
  };
}

export async function fetchPessoa(id: string): Promise<Person> {
  const { data } = await apiClient.get<BackendPersonDetail>(ENDPOINTS.pessoas.porId(id));
  return mapPersonFromApi(data);
}

interface BackendTimelineEntry {
  dateTime: string;
  type: string;
  unitName: string;
  details: string | null;
}
interface BackendTimelineResponse {
  personId: string;
  level: string;
  entries: BackendTimelineEntry[];
}

/**
 * Deriva o tipo de icone da timeline a partir do `Type` real do backend
 * ("Atendimento"/"Encaminhamento") — o backend não distingue enviado vs.
 * recebido, entrada de fila, falta ou acesso cross-unidade como eventos
 * próprios (gap documentado); tudo isso vira "ATENDIMENTO"/"ENCAMINHAMENTO_ENVIADO".
 */
function mapTimelineEventType(tipo: string): TimelineEventoTipo {
  return tipo === "Encaminhamento" ? "ENCAMINHAMENTO_ENVIADO" : "ATENDIMENTO";
}

/**
 * Busca a linha do tempo unificada da pessoa.
 *
 * O backend real não tem desbloqueio por-evento/por-secretaria: o parâmetro
 * `nivel` (metadados|completo) e a `justificativa` valem para a resposta
 * inteira (`GetPersonTimelineQueryHandler`). `completo`/`justificativa` aqui
 * refletem exatamente isso — uma vez concedido, todos os eventos cross-
 * secretaria da resposta vêm com `Details` preenchido, não só os da
 * secretaria que motivou o pedido (simplificação em relação ao design
 * original da UI, que previa cadeados por secretaria).
 */
export async function fetchTimeline(
  id: string,
  opts: { completo?: boolean; justificativa?: string } = {},
): Promise<TimelineResponse> {
  const [{ data }, unidades] = await Promise.all([
    apiClient.get<BackendTimelineResponse>(ENDPOINTS.pessoas.linhaDoTempo(id), {
      params: {
        nivel: opts.completo ? "completo" : "metadados",
        justificativa: opts.completo ? opts.justificativa : undefined,
      },
    }),
    fetchUnidades(),
  ]);

  const secretariatByUnitName = new Map(unidades.map((u) => [u.nome, u.secretariaResponsavel]));
  const locked = data.level !== "completo";

  const eventos: TimelineEvento[] = data.entries.map((entry, index) => ({
    id: `${id}-${index}-${entry.dateTime}`,
    tipo: mapTimelineEventType(entry.type),
    titulo: `${entry.type} — ${entry.unitName}`,
    descricao: entry.details ?? undefined,
    dataHora: entry.dateTime,
    unidadeSigla: entry.unitName,
    secretariat: mapSecretariatToApi(secretariatByUnitName.get(entry.unitName) ?? "SAUDE"),
    locked: locked && entry.details === null,
  }));

  return { eventos, hiddenCount: eventos.filter((e) => e.locked).length };
}

interface BackendCreatePersonResponse {
  personId: string;
  candidates: { id: string; name: string; birthDate: string; motherName?: string }[];
}

/**
 * O backend real nunca bloqueia a criação por duplicidade — sempre cria e,
 * se achar candidatos por nome+data de nascimento (RN02 camada 3), devolve
 * 200 com `candidates` preenchido (um `DuplicateAlert` já foi aberto para
 * cada um). `guardian`/`consentimentoLgpd` do request não têm equivalente no
 * `CreatePersonRequest` real (sem criação combinada de responsável, sem
 * checkbox de consentimento na criação) — são ignorados aqui; o cadastro de
 * responsável e de consentimentos LGPD é feito por chamadas separadas depois
 * (como o `CadastroPage.tsx` já faz para os demais perfis).
 */
export async function criarPessoa(request: CreatePersonRequest): Promise<DuplicateFoundResponse> {
  const { data } = await apiClient.post<BackendCreatePersonResponse>(ENDPOINTS.pessoas.base, {
    name: request.fullName,
    birthDate: request.birthDate,
    cns: request.cns,
    cpf: request.cpf,
    motherName: request.motherName,
    gender: request.gender,
    raceColor: request.raceColor,
    phone: request.phone,
    email: request.email,
    street: request.address?.street,
    number: request.address?.number,
    neighborhood: request.address?.neighborhood,
    city: request.address?.city,
    state: request.address?.state,
    zipCode: request.address?.zipCode,
    naturality: request.naturality,
    currentSchool: request.currentSchool,
    grade: request.grade,
    shift: request.shift,
    classGroup: request.classGroup,
    zone: request.zone,
    schoolEnrollment: request.schoolEnrollment,
    referredBySchool: request.referredBySchool,
    needsSpecialEducation: request.needsSpecialEducation,
    attendsTutoring: request.attendsTutoring,
    hasFailedGrade: request.hasFailedGrade,
    disabilityTypes: request.disabilityTypes,
  });

  return {
    id: data.personId,
    candidates: data.candidates.map((c) => ({ id: c.id, fullName: c.name, birthDate: c.birthDate, motherName: c.motherName })),
  };
}

/** `POST /api/pessoas/mesclar` — confirma que dois cadastros são a mesma pessoa (RF04/RN02). */
export async function mesclarPessoas(sourcePersonId: string, targetPersonId: string, duplicateAlertId?: string) {
  const { data } = await apiClient.post(ENDPOINTS.pessoas.mesclar, {
    sourcePersonId,
    targetPersonId,
    duplicateAlertId,
  });
  return data;
}

/** Backend real responde 404 (não 200 com corpo `null`) quando a pessoa ainda não tem perfil clínico preenchido. */
export async function fetchPerfilClinico(personId: string): Promise<PersonClinicalProfile | null> {
  try {
    const { data } = await apiClient.get<PersonClinicalProfile>(ENDPOINTS.pessoas.perfilClinico(personId));
    return data;
  } catch (error) {
    if (error instanceof NotFoundError) return null;
    throw error;
  }
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

/** Backend real responde 404 (não 200 com corpo `null`) quando a pessoa ainda não tem composição familiar preenchida. */
export async function fetchComposicaoFamiliar(personId: string): Promise<FamilyComposition | null> {
  try {
    const { data } = await apiClient.get<FamilyComposition>(ENDPOINTS.pessoas.composicaoFamiliar(personId));
    return data;
  } catch (error) {
    if (error instanceof NotFoundError) return null;
    throw error;
  }
}

export async function salvarComposicaoFamiliar(personId: string, request: UpsertFamilyCompositionRequest) {
  const { data } = await apiClient.put(ENDPOINTS.pessoas.composicaoFamiliar(personId), request);
  return data;
}

/** Backend real responde 404 (não 200 com corpo `null`) quando a pessoa ainda não tem marcos de desenvolvimento preenchidos. */
export async function fetchDesenvolvimento(personId: string): Promise<DevelopmentMilestones | null> {
  try {
    const { data } = await apiClient.get<DevelopmentMilestones>(ENDPOINTS.pessoas.desenvolvimento(personId));
    return data;
  } catch (error) {
    if (error instanceof NotFoundError) return null;
    throw error;
  }
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

/** Backend real exige `{justification}` no corpo do DELETE (RevokeConsentRequest) — antes não era enviado. */
export async function revogarConsentimento(personId: string, consentId: string, justification: string) {
  const { data } = await apiClient.delete(ENDPOINTS.pessoas.consentimento(personId, consentId), {
    data: { justification },
  });
  return data;
}

export interface SolicitarAcessoRequest {
  /** legalBasisId/purpose mantidos só para a UX do formulário (T15) — o backend real só aceita `justificativa`. */
  legalBasisId?: string;
  purpose?: string;
  justification: string;
}

/** Backend real (`SolicitarAcessoRequest`) só aceita `{justificativa}` — legalBasisId/purpose não são enviados (gap). */
export async function solicitarAcesso(personId: string, request: SolicitarAcessoRequest) {
  const { data } = await apiClient.post(ENDPOINTS.pessoas.solicitarAcesso(personId), {
    justificativa: request.justification,
  });
  return data;
}
