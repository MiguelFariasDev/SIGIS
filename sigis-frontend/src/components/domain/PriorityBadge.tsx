import { Badge } from "@/components/ui/badge";
import type { PrioridadeFila } from "@/lib/types/enums";
import { cn } from "@/lib/utils";
import { PRIORIDADE_COLOR, PRIORIDADE_LABEL } from "@/lib/utils/constants";

interface PriorityBadgeProps {
  prioridade: PrioridadeFila;
  className?: string;
}

export function PriorityBadge({ prioridade, className }: PriorityBadgeProps) {
  return (
    <Badge variant="outline" className={cn("font-medium", PRIORIDADE_COLOR[prioridade], className)}>
      {PRIORIDADE_LABEL[prioridade]}
    </Badge>
  );
}
