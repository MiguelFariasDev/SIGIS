export const PrioridadeFila = {
  URGENTE: "URGENTE",
  CURTO_PRAZO: "CURTO_PRAZO",
  LISTA_ESPERA: "LISTA_ESPERA",
} as const;
export type PrioridadeFila = (typeof PrioridadeFila)[keyof typeof PrioridadeFila];

export const StatusFila = {
  AGUARDANDO: "AGUARDANDO",
  EM_ATENDIMENTO: "EM_ATENDIMENTO",
  CONCLUIDO: "CONCLUIDO",
  FALTOU: "FALTOU",
  BUSCA_ATIVA: "BUSCA_ATIVA",
} as const;
export type StatusFila = (typeof StatusFila)[keyof typeof StatusFila];

export const StatusComparecimento = {
  AGENDADO: "AGENDADO",
  COMPARECEU: "COMPARECEU",
  FALTOU: "FALTOU",
} as const;
export type StatusComparecimento = (typeof StatusComparecimento)[keyof typeof StatusComparecimento];

export const TipoSessao = {
  ANAMNESE_PSI: "ANAMNESE_PSI",
  ANAMNESE_PSICOPED: "ANAMNESE_PSICOPED",
  INSTRUMENTAL_EDFISICA: "INSTRUMENTAL_EDFISICA",
  PRONTUARIO_NASF: "PRONTUARIO_NASF",
  SINTESE: "SINTESE",
} as const;
export type TipoSessao = (typeof TipoSessao)[keyof typeof TipoSessao];

export const StatusEncaminhamento = {
  PENDENTE: "PENDENTE",
  ACEITO: "ACEITO",
  CONCLUIDO: "CONCLUIDO",
  RECUSADO: "RECUSADO",
} as const;
export type StatusEncaminhamento = (typeof StatusEncaminhamento)[keyof typeof StatusEncaminhamento];

export const StatusAlertaDuplicidade = {
  PENDENTE: "PENDENTE",
  MESCLADO: "MESCLADO",
  FALSO_POSITIVO: "FALSO_POSITIVO",
} as const;
export type StatusAlertaDuplicidade = (typeof StatusAlertaDuplicidade)[keyof typeof StatusAlertaDuplicidade];

export const PapelRbac = {
  PROFISSIONAL: "PROFISSIONAL",
  COORDENADOR: "COORDENADOR",
  AUDITOR: "AUDITOR",
} as const;
export type PapelRbac = (typeof PapelRbac)[keyof typeof PapelRbac];

export const SecretariaResponsavel = {
  SAUDE: "SAUDE",
  EDUCACAO: "EDUCACAO",
  ASSISTENCIA: "ASSISTENCIA",
} as const;
export type SecretariaResponsavel = (typeof SecretariaResponsavel)[keyof typeof SecretariaResponsavel];

// Reexporta os tipos das novas entidades (facetas Saude/Educacao) para
// manter um ponto unico de consulta de enums/uniões do dominio.
export type { SchoolStatus } from "./schoolHistory";
export type { ManualDominance } from "./developmentMilestones";
export type { LearningDifficultyType, LearningDifficultySeverity } from "./learningDifficulties";
export type { ConsentType } from "./personConsent";
export type { DayOfWeek } from "./concurrentTreatment";
export type { Secretariat } from "./legalBasis";
