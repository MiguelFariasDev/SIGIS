import type { Secretariat } from "./legalBasis";

export type TimelineEventoTipo =
  | "CADASTRO"
  | "ATENDIMENTO"
  | "ENCAMINHAMENTO_ENVIADO"
  | "ENCAMINHAMENTO_RECEBIDO"
  | "ENTRADA_FILA"
  | "FALTA"
  | "ACESSO_CROSS_UNIDADE";

export interface TimelineEvento {
  id: string;
  tipo: TimelineEventoTipo;
  titulo: string;
  descricao?: string;
  dataHora: string; // ISO timestamp
  unidadeSigla?: string;
  profissionalNome?: string;
  acessoRegistrado?: boolean; // true quando cross-unidade (RF12/RF13)
  /** Secretaria de origem do evento — define a cor exibida na timeline. */
  secretariat: Secretariat;
  /**
   * true quando o evento e cross-secretaria e o profissional atual ainda
   * nao tem acesso ao conteudo (RF12) — a timeline mostra o cadeado e
   * oculta descricao/detalhes ate a solicitacao de acesso ser aprovada.
   */
  locked?: boolean;
}

export interface TimelineResponse {
  eventos: TimelineEvento[];
  hiddenCount: number;
}
