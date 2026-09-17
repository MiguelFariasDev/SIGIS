import type { SecretariaResponsavel } from "./enums";

export interface UnidadeServico {
  id: string;
  nome: string;
  sigla: string;
  secretariaResponsavel: SecretariaResponsavel;
}
