import { useDroppable } from "@dnd-kit/core";
import type { StatusFila } from "@/lib/types/enums";
import type { FilaAtendimento } from "@/lib/types/fila";
import { cn } from "@/lib/utils";
import { FilaItem } from "./FilaItem";

interface KanbanColunaProps {
  /** Identificador do droppable — status quando `droppable=true`, especialidade caso contrario. */
  id: string;
  titulo: string;
  itens: FilaAtendimento[];
  onAtualizarStatus: (id: string, status: StatusFila) => void;
  mostrarUnidade?: boolean;
  /** false na visao "por especialidade" — colunas nao aceitam drag-and-drop de status. */
  droppable?: boolean;
}

export function KanbanColuna({ id, titulo, itens, onAtualizarStatus, mostrarUnidade, droppable = true }: KanbanColunaProps) {
  const { setNodeRef, isOver } = useDroppable({ id, disabled: !droppable });

  return (
    <div
      ref={setNodeRef}
      className={cn(
        "flex min-h-[300px] w-full flex-col rounded-xl border border-border bg-neutral-50 p-3 transition-colors",
        droppable && isOver && "border-sus-blue bg-sus-blue-light/40",
      )}
    >
      <div className="mb-3 flex items-center justify-between px-1">
        <p className="text-sm font-semibold text-foreground">{titulo}</p>
        <span className="rounded-full bg-white px-2 py-0.5 text-xs font-medium text-muted-foreground">
          {itens.length}
        </span>
      </div>
      <div className="flex-1 space-y-2">
        {itens.map((item) => (
          <FilaItem key={item.id} item={item} onAtualizarStatus={onAtualizarStatus} mostrarUnidade={mostrarUnidade} />
        ))}
      </div>
    </div>
  );
}
