import type { StatusAlertaDuplicidade } from "./enums";
import type { Person } from "./person";

export interface AlertaDuplicidade {
  id: string;
  personId1: string;
  personId2: string;
  person1?: Person;
  person2?: Person;
  scoreSimilaridade: number; // 0..1
  motivoMatch: string;
  status: StatusAlertaDuplicidade;
  resolvidoPorProfissionalId?: string;
  resolvidoPorProfissionalNome?: string;
  resolvidoEm?: string;
  criadoEm: string;
}

export interface ResolverDuplicidadeRequest {
  acao: "MESCLAR" | "FALSO_POSITIVO";
}
