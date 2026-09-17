import type { Secretariat } from "./legalBasis";

export interface LogAcesso {
  id: string;
  personId: string;
  personName: string;
  profissionalId: string;
  profissionalNome: string;
  acao: string;
  baseLegal: string;
  legalBasisId?: string;
  justificativa?: string;
  secretariat?: Secretariat;
  dataHora: string; // ISO timestamp
  crossUnidade: boolean;
}

export interface AuditoriaFiltros {
  personId?: string;
  profissionalId?: string;
  dataInicio?: string;
  dataFim?: string;
  acao?: string;
  secretariat?: Secretariat;
  crossUnidadeApenas?: boolean;
}
