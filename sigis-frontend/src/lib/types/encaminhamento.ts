import type { PrioridadeFila, StatusEncaminhamento } from "./enums";

export interface Encaminhamento {
  id: string;
  personId: string;
  personName: string;
  unidadeOrigemId: string;
  unidadeOrigemSigla: string;
  unidadeDestinoId: string;
  unidadeDestinoSigla: string;
  motivo: string;
  prioridade: PrioridadeFila;
  dataEncaminhamento: string; // ISO timestamp
  status: StatusEncaminhamento;
  aceitoPor?: string;
  aceitoEm?: string;
  primeiroAtendimentoEm?: string;
  recusaMotivo?: string;
  legalBasisId?: string;
}

export interface CriarEncaminhamentoRequest {
  personId: string;
  unidadeOrigemId: string;
  unidadeDestinoId: string;
  motivo: string;
  prioridade: PrioridadeFila;
  legalBasisId?: string;
}

export interface RecusarEncaminhamentoRequest {
  motivo: string;
}
