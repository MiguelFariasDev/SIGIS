import type { PrioridadeFila, StatusFila } from "./enums";

export interface FilaAtendimento {
  id: string;
  personId: string;
  personName: string;
  personBirthDate: string;
  unidadeId: string;
  especialidade: string;
  prioridade: PrioridadeFila;
  entradaFila: string; // ISO timestamp
  status: StatusFila;
}

export interface CriarFilaRequest {
  personId: string;
  unidadeId: string;
  especialidade: string;
  prioridade: PrioridadeFila;
}

export interface AtualizarStatusFilaRequest {
  status: StatusFila;
}
