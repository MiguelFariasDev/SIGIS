/**
 * Traduz os valores de enum retornados pelo backend real (inglês,
 * PascalCase) para os valores em português usados pelos tipos internos do
 * frontend (construídos originalmente contra os mocks MSW). Mantido em um
 * único módulo para que cada `features/*\/api.ts` faça a tradução no limite
 * da API, sem espalhar tabelas de mapeamento pelo app.
 */
import {
  PapelRbac,
  PrioridadeFila,
  SecretariaResponsavel,
  StatusComparecimento,
  StatusEncaminhamento,
  StatusFila,
  TipoSessao,
} from "@/lib/types/enums";
import type { Secretariat } from "@/lib/types/legalBasis";

function buildReverseMap<T extends Record<string, string>>(map: T): Record<string, keyof T> {
  const reverse: Record<string, keyof T> = {} as Record<string, keyof T>;
  for (const key in map) reverse[map[key]] = key;
  return reverse;
}

const PRIORITY_TO_PT: Record<string, PrioridadeFila> = {
  Urgent: PrioridadeFila.URGENTE,
  ShortTerm: PrioridadeFila.CURTO_PRAZO,
  WaitingList: PrioridadeFila.LISTA_ESPERA,
};
const PRIORITY_TO_EN = buildReverseMap(PRIORITY_TO_PT as unknown as Record<string, string>);

const QUEUE_STATUS_TO_PT: Record<string, StatusFila> = {
  Waiting: StatusFila.AGUARDANDO,
  InAttendance: StatusFila.EM_ATENDIMENTO,
  Completed: StatusFila.CONCLUIDO,
  Absent: StatusFila.FALTOU,
  ActiveSearch: StatusFila.BUSCA_ATIVA,
};

const ATTENDANCE_STATUS_TO_PT: Record<string, StatusComparecimento> = {
  Scheduled: StatusComparecimento.AGENDADO,
  Attended: StatusComparecimento.COMPARECEU,
  Absent: StatusComparecimento.FALTOU,
};

const SESSION_TYPE_TO_PT: Record<string, TipoSessao> = {
  PsychologicalAnamnesis: TipoSessao.ANAMNESE_PSI,
  PsychopedagogicalAnamnesis: TipoSessao.ANAMNESE_PSICOPED,
  PhysicalEducationInstrument: TipoSessao.INSTRUMENTAL_EDFISICA,
  NasfRecord: TipoSessao.PRONTUARIO_NASF,
  Synthesis: TipoSessao.SINTESE,
};
const SESSION_TYPE_TO_EN = buildReverseMap(SESSION_TYPE_TO_PT as unknown as Record<string, string>);

const REFERRAL_STATUS_TO_PT: Record<string, StatusEncaminhamento> = {
  Pending: StatusEncaminhamento.PENDENTE,
  Accepted: StatusEncaminhamento.ACEITO,
  Completed: StatusEncaminhamento.CONCLUIDO,
  Refused: StatusEncaminhamento.RECUSADO,
};

const RBAC_ROLE_TO_PT: Record<string, PapelRbac> = {
  Professional: PapelRbac.PROFISSIONAL,
  Coordinator: PapelRbac.COORDENADOR,
  Auditor: PapelRbac.AUDITOR,
};

const SECRETARIAT_TO_PT: Record<string, SecretariaResponsavel> = {
  Health: SecretariaResponsavel.SAUDE,
  Education: SecretariaResponsavel.EDUCACAO,
  SocialAssistance: SecretariaResponsavel.ASSISTENCIA,
};
const SECRETARIAT_TO_EN = buildReverseMap(SECRETARIAT_TO_PT as unknown as Record<string, string>);

/** Converte com fallback: valor desconhecido é logado e devolvido como veio, para não quebrar a UI. */
function translate<T extends string>(map: Record<string, T>, value: string, label: string): T {
  const mapped = map[value];
  if (mapped === undefined) {
    console.warn(`[backendMappers] valor de ${label} não mapeado: "${value}"`);
    return value as T;
  }
  return mapped;
}

export const mapPriorityFromApi = (v: string) => translate(PRIORITY_TO_PT, v, "prioridade");
export const mapPriorityToApi = (v: PrioridadeFila) => PRIORITY_TO_EN[v] ?? v;
export const mapQueueStatusFromApi = (v: string) => translate(QUEUE_STATUS_TO_PT, v, "status de fila");
export const mapAttendanceStatusFromApi = (v: string) => translate(ATTENDANCE_STATUS_TO_PT, v, "status de comparecimento");
export const mapSessionTypeFromApi = (v: string) => translate(SESSION_TYPE_TO_PT, v, "tipo de sessão");
export const mapSessionTypeToApi = (v: TipoSessao) => SESSION_TYPE_TO_EN[v] ?? v;
export const mapReferralStatusFromApi = (v: string) => translate(REFERRAL_STATUS_TO_PT, v, "status de encaminhamento");
export const mapRbacRoleFromApi = (v: string) => translate(RBAC_ROLE_TO_PT, v, "papel RBAC");
export const mapSecretariatFromApi = (v: string) => translate(SECRETARIAT_TO_PT, v, "secretaria");
/** Inverso de {@link mapSecretariatFromApi} — usado quando um valor já convertido para PT (ex.: `UnidadeServico.secretariaResponsavel`) precisa voltar ao inglês do backend (ex.: `TimelineEvento.secretariat`). */
export const mapSecretariatToApi = (v: SecretariaResponsavel): Secretariat =>
  (SECRETARIAT_TO_EN[v] ?? v) as Secretariat;
