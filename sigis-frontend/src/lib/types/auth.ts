import type { PapelRbac } from "./enums";

export interface LoginRequest {
  email: string;
  senha: string;
  lembrarMe?: boolean;
}

export interface LoginResponse {
  token: string;
  profissional: {
    id: string;
    nome: string;
    email: string;
    unidadeId: string;
    unidadeSigla: string;
    papelRbac: PapelRbac;
  };
}
