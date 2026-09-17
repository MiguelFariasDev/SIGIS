import { Check, Clock, Send, Stethoscope } from "lucide-react";
import type { Encaminhamento } from "@/lib/types/encaminhamento";
import { formatarDataHora } from "@/lib/utils/formatters";
import { cn } from "@/lib/utils";

interface RastreioTimelineProps {
  encaminhamento: Encaminhamento;
}

export function RastreioTimeline({ encaminhamento }: RastreioTimelineProps) {
  const etapas = [
    {
      titulo: `Encaminhado por ${encaminhamento.unidadeOrigemSigla}`,
      dataHora: encaminhamento.dataEncaminhamento,
      icone: Send,
      concluida: true,
    },
    {
      titulo: encaminhamento.aceitoPor
        ? `Aceito por ${encaminhamento.aceitoPor} (${encaminhamento.unidadeDestinoSigla})`
        : `Aguardando aceite de ${encaminhamento.unidadeDestinoSigla}`,
      dataHora: encaminhamento.aceitoEm,
      icone: Check,
      concluida: !!encaminhamento.aceitoEm,
    },
    {
      titulo: encaminhamento.primeiroAtendimentoEm ? "Primeiro atendimento realizado" : "Primeiro atendimento pendente",
      dataHora: encaminhamento.primeiroAtendimentoEm,
      icone: Stethoscope,
      concluida: !!encaminhamento.primeiroAtendimentoEm,
    },
  ];

  return (
    <div className="rounded-xl border border-border bg-white p-6">
      <div className="flex flex-col gap-6 sm:flex-row sm:items-start sm:justify-between">
        {etapas.map((etapa, index) => (
          <div key={etapa.titulo} className="flex flex-1 items-start gap-3">
            <div className="flex flex-col items-center">
              <div
                className={cn(
                  "flex size-9 shrink-0 items-center justify-center rounded-full",
                  etapa.concluida ? "bg-sus-green-light text-sus-green" : "bg-neutral-100 text-neutral-400",
                )}
              >
                <etapa.icone className="size-4" />
              </div>
              {index < etapas.length - 1 && (
                <div className="mt-1 hidden h-px w-full min-w-8 flex-1 bg-border sm:block" />
              )}
            </div>
            <div className="min-w-0">
              <p className="text-sm font-medium text-foreground">{etapa.titulo}</p>
              <p className="mt-0.5 flex items-center gap-1 text-xs text-muted-foreground">
                <Clock className="size-3" />
                {etapa.dataHora ? formatarDataHora(etapa.dataHora) : "Pendente"}
              </p>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}
