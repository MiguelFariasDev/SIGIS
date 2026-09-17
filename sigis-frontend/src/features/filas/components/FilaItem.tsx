import { useDraggable } from "@dnd-kit/core";
import { CheckCircle, Clock, UserX } from "lucide-react";
import { useNavigate } from "react-router-dom";
import { PriorityBadge } from "@/components/domain/PriorityBadge";
import { Button } from "@/components/ui/button";
import { unidadesMock } from "@/lib/mocks/data/unidades.mock";
import { StatusFila } from "@/lib/types/enums";
import type { FilaAtendimento } from "@/lib/types/fila";
import { formatarDistancia, formatarIdade } from "@/lib/utils/formatters";
import { cn } from "@/lib/utils";

interface FilaItemProps {
  item: FilaAtendimento;
  onAtualizarStatus: (id: string, status: StatusFila) => void;
  /** Exibe a sigla da unidade — usado na visao "rede inteira" (T07 nivel 1). */
  mostrarUnidade?: boolean;
}

export function FilaItem({ item, onAtualizarStatus, mostrarUnidade }: FilaItemProps) {
  const navigate = useNavigate();
  const { attributes, listeners, setNodeRef, transform, isDragging } = useDraggable({ id: item.id });

  const style = transform
    ? { transform: `translate3d(${transform.x}px, ${transform.y}px, 0)` }
    : undefined;

  const unidade = mostrarUnidade ? unidadesMock.find((u) => u.id === item.unidadeId) : undefined;

  return (
    <div
      ref={setNodeRef}
      style={style}
      {...attributes}
      {...listeners}
      className={cn(
        "cursor-grab space-y-2 rounded-lg border border-border bg-white p-3 shadow-sm active:cursor-grabbing",
        isDragging && "z-10 opacity-70 shadow-md",
      )}
    >
      <div className="flex items-start justify-between gap-2">
        <button
          type="button"
          className="text-left text-sm font-medium text-foreground hover:underline"
          onClick={() => navigate(`/pacientes/${item.personId}`)}
          onPointerDown={(e) => e.stopPropagation()}
        >
          {item.personName}
        </button>
        <PriorityBadge prioridade={item.prioridade} />
      </div>
      <p className="text-xs text-muted-foreground">
        {formatarIdade(item.personBirthDate)} — {item.especialidade}
        {unidade && ` — ${unidade.sigla}`}
      </p>
      <p className="flex items-center gap-1 text-xs text-muted-foreground/80">
        <Clock className="size-3" />
        na fila {formatarDistancia(item.entradaFila)}
      </p>

      {item.status !== StatusFila.CONCLUIDO && item.status !== StatusFila.FALTOU && (
        <div className="flex gap-1.5 pt-1" onPointerDown={(e) => e.stopPropagation()}>
          <Button
            size="sm"
            variant="outline"
            className="h-7 flex-1 text-xs"
            onClick={() => onAtualizarStatus(item.id, StatusFila.CONCLUIDO)}
          >
            <CheckCircle className="size-3.5" />
            Compareceu
          </Button>
          <Button
            size="sm"
            variant="outline"
            className="h-7 flex-1 text-xs text-sus-red hover:text-sus-red"
            onClick={() => onAtualizarStatus(item.id, StatusFila.FALTOU)}
          >
            <UserX className="size-3.5" />
            Faltou
          </Button>
        </div>
      )}
    </div>
  );
}
