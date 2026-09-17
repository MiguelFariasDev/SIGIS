import type { PapelRbac } from "./enums";

export interface Profissional {
  id: string;
  nome: string;
  especialidade: string;
  unidadeId: string;
  unidadeSigla?: string;
  papelRbac: PapelRbac;
  email: string;
  lastLoginAt?: string;
}
