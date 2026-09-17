import {
  PapelRbac,
  PrioridadeFila,
  SecretariaResponsavel,
  StatusAlertaDuplicidade,
  StatusComparecimento,
  StatusEncaminhamento,
  StatusFila,
  TipoSessao,
} from "@/lib/types/enums";
import type { Secretariat } from "@/lib/types/legalBasis";

export const PRIORIDADE_LABEL: Record<PrioridadeFila, string> = {
  [PrioridadeFila.URGENTE]: "Urgente",
  [PrioridadeFila.CURTO_PRAZO]: "Curto prazo",
  [PrioridadeFila.LISTA_ESPERA]: "Lista de espera",
};

export const PRIORIDADE_COLOR: Record<PrioridadeFila, string> = {
  [PrioridadeFila.URGENTE]: "bg-sus-red-light text-sus-red-dark border-sus-red/30",
  [PrioridadeFila.CURTO_PRAZO]: "bg-sus-yellow-light text-sus-yellow-dark border-sus-yellow/40",
  [PrioridadeFila.LISTA_ESPERA]: "bg-neutral-100 text-neutral-600 border-neutral-300",
};

export const STATUS_FILA_LABEL: Record<StatusFila, string> = {
  [StatusFila.AGUARDANDO]: "Aguardando",
  [StatusFila.EM_ATENDIMENTO]: "Em atendimento",
  [StatusFila.CONCLUIDO]: "Concluido",
  [StatusFila.FALTOU]: "Faltou",
  [StatusFila.BUSCA_ATIVA]: "Busca ativa",
};

export const STATUS_FILA_COLOR: Record<StatusFila, string> = {
  [StatusFila.AGUARDANDO]: "bg-sus-blue-light text-sus-blue-dark border-sus-blue/30",
  [StatusFila.EM_ATENDIMENTO]: "bg-sus-green-light text-sus-green-dark border-sus-green/30",
  [StatusFila.CONCLUIDO]: "bg-neutral-100 text-neutral-600 border-neutral-300",
  [StatusFila.FALTOU]: "bg-sus-red-light text-sus-red-dark border-sus-red/30",
  [StatusFila.BUSCA_ATIVA]: "bg-sus-yellow-light text-sus-yellow-dark border-sus-yellow/40",
};

export const COMPARECIMENTO_LABEL: Record<StatusComparecimento, string> = {
  [StatusComparecimento.AGENDADO]: "Agendado",
  [StatusComparecimento.COMPARECEU]: "Compareceu",
  [StatusComparecimento.FALTOU]: "Faltou",
};

export const STATUS_ENCAMINHAMENTO_LABEL: Record<StatusEncaminhamento, string> = {
  [StatusEncaminhamento.PENDENTE]: "Pendente",
  [StatusEncaminhamento.ACEITO]: "Aceito",
  [StatusEncaminhamento.CONCLUIDO]: "Concluido",
  [StatusEncaminhamento.RECUSADO]: "Recusado",
};

export const STATUS_ENCAMINHAMENTO_COLOR: Record<StatusEncaminhamento, string> = {
  [StatusEncaminhamento.PENDENTE]: "bg-sus-yellow-light text-sus-yellow-dark border-sus-yellow/40",
  [StatusEncaminhamento.ACEITO]: "bg-sus-blue-light text-sus-blue-dark border-sus-blue/30",
  [StatusEncaminhamento.CONCLUIDO]: "bg-sus-green-light text-sus-green-dark border-sus-green/30",
  [StatusEncaminhamento.RECUSADO]: "bg-sus-red-light text-sus-red-dark border-sus-red/30",
};

export const STATUS_DUPLICIDADE_LABEL: Record<StatusAlertaDuplicidade, string> = {
  [StatusAlertaDuplicidade.PENDENTE]: "Pendente",
  [StatusAlertaDuplicidade.MESCLADO]: "Mesclado",
  [StatusAlertaDuplicidade.FALSO_POSITIVO]: "Falso positivo",
};

export const TIPO_SESSAO_LABEL: Record<TipoSessao, string> = {
  [TipoSessao.ANAMNESE_PSI]: "Anamnese psicologica",
  [TipoSessao.ANAMNESE_PSICOPED]: "Anamnese psicopedagogica",
  [TipoSessao.INSTRUMENTAL_EDFISICA]: "Instrumental de educacao fisica",
  [TipoSessao.PRONTUARIO_NASF]: "Prontuario NASF",
  [TipoSessao.SINTESE]: "Sintese de acompanhamento",
};

export const PAPEL_LABEL: Record<PapelRbac, string> = {
  [PapelRbac.PROFISSIONAL]: "Profissional",
  [PapelRbac.COORDENADOR]: "Coordenador(a)",
  [PapelRbac.AUDITOR]: "Auditor(a) / DPO",
};

// Paleta oficial por secretaria (RN — nao alterar sem alinhar com o time):
// Saude = azul, Educacao = amarelo, Assistencia Social = verde.
export const SECRETARIA_COLOR: Record<SecretariaResponsavel, string> = {
  [SecretariaResponsavel.SAUDE]: "bg-sus-blue-light text-sus-blue-dark border-sus-blue/30",
  [SecretariaResponsavel.EDUCACAO]: "bg-sus-yellow-light text-sus-yellow-dark border-sus-yellow/40",
  [SecretariaResponsavel.ASSISTENCIA]: "bg-sus-green-light text-sus-green-dark border-sus-green/30",
};

export const SERVICOS_SIGLAS = ["NASF", "CREAES", "NAPE", "CASA_MAIS_AZUL", "CRASF"] as const;
export type ServicoSigla = (typeof SERVICOS_SIGLAS)[number];

export const SERVICO_LABEL: Record<ServicoSigla, string> = {
  NASF: "NASF",
  CREAES: "CREAES",
  NAPE: "NAPE",
  CASA_MAIS_AZUL: "Casa Mais Azul",
  CRASF: "CRASF",
};

export const SERVICO_SECRETARIA: Record<ServicoSigla, SecretariaResponsavel> = {
  NASF: SecretariaResponsavel.SAUDE,
  CREAES: SecretariaResponsavel.SAUDE,
  NAPE: SecretariaResponsavel.EDUCACAO,
  CASA_MAIS_AZUL: SecretariaResponsavel.SAUDE,
  CRASF: SecretariaResponsavel.ASSISTENCIA,
};

export const SERVICO_SECRETARIAT: Record<ServicoSigla, Secretariat> = {
  NASF: "Health",
  CREAES: "Health",
  NAPE: "Education",
  CASA_MAIS_AZUL: "Health",
  CRASF: "SocialAssistance",
};

export const SECRETARIAT_LABEL: Record<Secretariat, string> = {
  Health: "Saude",
  Education: "Educacao",
  SocialAssistance: "Assistencia Social",
};

export const SECRETARIAT_EMOJI: Record<Secretariat, string> = {
  Health: "🟦",
  Education: "🟨",
  SocialAssistance: "🟩",
};

export const SECRETARIAT_COLOR: Record<Secretariat, string> = {
  Health: "bg-sus-blue-light text-sus-blue-dark border-sus-blue/30",
  Education: "bg-sus-yellow-light text-sus-yellow-dark border-sus-yellow/40",
  SocialAssistance: "bg-sus-green-light text-sus-green-dark border-sus-green/30",
};
