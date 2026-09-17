import { PrioridadeFila, StatusEncaminhamento } from "@/lib/types/enums";
import type { Encaminhamento } from "@/lib/types/encaminhamento";

export const encaminhamentosMock: Encaminhamento[] = [
  {
    id: "encaminhamento-01",
    personId: "pessoa-10",
    personName: "Valentina Alves Barbosa",
    unidadeOrigemId: "unidade-nasf",
    unidadeOrigemSigla: "NASF",
    unidadeDestinoId: "unidade-nape",
    unidadeDestinoSigla: "NAPE",
    motivo: "Encaminhamento para avaliacao pedagogica especializada apos triagem no NASF.",
    prioridade: PrioridadeFila.CURTO_PRAZO,
    dataEncaminhamento: "2025-09-01T09:00:00Z",
    status: StatusEncaminhamento.ACEITO,
    aceitoPor: "Juliana Pinheiro Andrade",
    aceitoEm: "2025-09-10T11:00:00Z",
    primeiroAtendimentoEm: "2025-09-15T10:00:00Z",
  },
  {
    id: "encaminhamento-02",
    personId: "pessoa-06",
    personName: "Gabriel Santos Pereira",
    unidadeOrigemId: "unidade-nasf",
    unidadeOrigemSigla: "NASF",
    unidadeDestinoId: "unidade-nape",
    unidadeDestinoSigla: "NAPE",
    motivo: "Suspeita de TEA identificada em consulta de rotina; solicita avaliacao psicologica.",
    prioridade: PrioridadeFila.URGENTE,
    dataEncaminhamento: "2026-02-15T13:20:00Z",
    status: StatusEncaminhamento.PENDENTE,
  },
];
