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
  /** Endereço formatado em uma linha — o backend real não devolve os componentes separados na leitura. */
  address?: string;

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

  /** Siglas dos servicos com historico (derivado, uso em UI). Não preenchido pelo backend real ainda. */
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
  /** Endereço estruturado — só aceito na criação (o backend expõe campos soltos: street/number/... ). */
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
  /**
   * Não suportados pelo real `CreatePersonRequest` do backend (sem endpoint
   * de responsável/consentimento combinado na criação, e a criação nunca é
   * bloqueada por duplicidade — sempre cria, e sinaliza candidatos à parte).
   * Mantidos aqui apenas para não quebrar o formulário existente; a camada
   * de API (`features/pacientes/api.ts`) os ignora ao montar a requisição.
   */
  guardian?: CreateGuardianRequest;
  consentimentoLgpd: boolean;
}

export interface CreateGuardianRequest {
  name: string;
  cns?: string;
  birthDate?: string;
  relationship: string;
}

/** Resumo leve de pessoa — usado em resultados de busca e em candidatos a duplicidade (mesmo shape do backend real). */
export interface PersonSearchResult {
  id: string;
  fullName: string;
  birthDate: string;
  motherName?: string;
  /**
   * services/queueStatus/priority: não retornados pelo endpoint real de
   * busca (`GET /api/pessoas/busca` só devolve id/name/birthDate/motherName)
   * — mantidos opcionais para a tabela de resultados continuar renderizando
   * as colunas (vazias) sem quebrar; gap documentado.
   */
  services?: string[];
  queueStatus?: string;
  priority?: string;
}

export interface PersonSearchFilters {
  term?: string;
  /**
   * Não suportados pelo endpoint real de busca (`GET /api/pessoas/busca`,
   * que só aceita `termo`/`limit`) — mantidos no tipo para não quebrar a UI
   * de filtros, mas ignorados pela camada de API por ora (gap documentado).
   */
  service?: string;
  status?: string;
  priority?: string;
}

/** Candidato a duplicidade — mesmo shape retornado por `CreatePersonResponse.Candidates` (sem score; o backend não expõe similaridade nesta resposta). */
export type DuplicateCandidate = PersonSearchResult;

export interface DuplicateFoundResponse {
  id: string;
  candidates: DuplicateCandidate[];
}
