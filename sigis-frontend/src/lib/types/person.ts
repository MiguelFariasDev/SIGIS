import type { Address } from "./address";

/**
 * Nucleo de identidade canonica da pessoa — compartilhado pelas facetas
 * de Saude e Educacao. Substitui o antigo tipo `Pessoa` (renomeado para
 * refletir que a pessoa pode ser somente aluno, somente paciente, ou
 * ambos, dependendo de quais perfis/registros existem para ela).
 */
export interface Person {
  id: string;
  fullName: string;
  birthDate: string;
  cns?: string;
  cpf?: string;
  motherName?: string;
  gender?: string;
  raceColor?: string;
  phone?: string;
  email?: string;
  address?: Address;

  // Faceta Educacao
  naturality?: string;
  currentSchool?: string;
  grade?: string;
  shift?: string;
  classGroup?: string;
  zone?: "Urbana" | "Rural";
  schoolEnrollment?: string;
  referredBySchool?: boolean;
  needsSpecialEducation?: boolean;
  attendsTutoring?: boolean;
  hasFailedGrade?: boolean;
  disabilityTypes?: string;

  createdAt: string;
  updatedAt: string;

  /** Siglas dos servicos com historico (derivado, uso em UI). */
  services?: string[];
}

export interface CreatePersonRequest {
  fullName: string;
  birthDate: string;
  cns?: string;
  cpf?: string;
  motherName?: string;
  gender?: string;
  raceColor?: string;
  phone?: string;
  email?: string;
  address?: Address;
  naturality?: string;
  currentSchool?: string;
  grade?: string;
  shift?: string;
  classGroup?: string;
  zone?: "Urbana" | "Rural";
  schoolEnrollment?: string;
  referredBySchool?: boolean;
  needsSpecialEducation?: boolean;
  attendsTutoring?: boolean;
  hasFailedGrade?: boolean;
  disabilityTypes?: string;
  guardian?: CreateGuardianRequest;
  consentimentoLgpd: boolean;
  forceCreateDespiteDuplicate?: boolean;
}

export interface CreateGuardianRequest {
  name: string;
  cns?: string;
  birthDate?: string;
  relationship: string;
}

export interface PersonSearchResult {
  id: string;
  fullName: string;
  birthDate: string;
  cns?: string;
  cpf?: string;
  services: string[];
  queueStatus?: string;
  priority?: string;
}

export interface PersonSearchFilters {
  term?: string;
  service?: string;
  status?: string;
  priority?: string;
}

export interface DuplicateCandidate {
  person: Person;
  score: number;
}

export interface DuplicateFoundResponse {
  message: string;
  candidates: DuplicateCandidate[];
}
