import { Clock, EyeOff } from "lucide-react";
import { useState } from "react";
import { EmptyState } from "@/components/common/EmptyState";
import type { TimelineEvento } from "@/lib/types/timeline";
import type { Secretariat } from "@/lib/types/legalBasis";
import { RequestAccessModal } from "./RequestAccessModal";
import { TimelineItem } from "./TimelineItem";

interface TimelineProps {
  eventos: TimelineEvento[];
  /** Quantidade de eventos cross-secretaria ainda ocultos (nao contados em `eventos` liberados). */
  hiddenCount?: number;
  personId: string;
  /** Concedido pelo backend real de forma global (não por secretaria) — recebe a justificativa usada. */
  onAccessGranted?: (justificativa: string) => void;
}

export function Timeline({ eventos, hiddenCount = 0, personId, onAccessGranted }: TimelineProps) {
  const [secretariaSolicitada, setSecretariaSolicitada] = useState<Secretariat | null>(null);

  if (eventos.length === 0) {
    return <EmptyState icon={Clock} title="Sem eventos registrados" description="Ainda nao ha historico para esta pessoa." />;
  }

  return (
    <div>
      {hiddenCount > 0 && (
        <div className="mb-4 flex items-center gap-2 rounded-lg border border-neutral-300 bg-neutral-50 px-3 py-2 text-xs text-muted-foreground">
          <EyeOff className="size-3.5" />
          {hiddenCount} evento(s) de outra(s) secretaria(s) estao ocultos nesta linha do tempo (RF12).
        </div>
      )}

      {eventos.map((evento, index) => (
        <TimelineItem
          key={evento.id}
          evento={evento}
          isLast={index === eventos.length - 1}
          onRequestAccess={evento.locked ? () => setSecretariaSolicitada(evento.secretariat) : undefined}
        />
      ))}

      {secretariaSolicitada && (
        <RequestAccessModal
          open={!!secretariaSolicitada}
          onOpenChange={(open) => !open && setSecretariaSolicitada(null)}
          personId={personId}
          secretariat={secretariaSolicitada}
          onGranted={(_secretariat, justificativa) => {
            onAccessGranted?.(justificativa);
            setSecretariaSolicitada(null);
          }}
        />
      )}
    </div>
  );
}
