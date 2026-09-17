import { Filter } from "lucide-react";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { PrioridadeFila, StatusFila } from "@/lib/types/enums";
import type { PersonSearchFilters } from "@/lib/types/person";
import { PRIORIDADE_LABEL, SERVICOS_SIGLAS, SERVICO_LABEL, STATUS_FILA_LABEL } from "@/lib/utils/constants";

interface FiltrosBuscaProps {
  filtros: PersonSearchFilters;
  onChange: (filtros: PersonSearchFilters) => void;
}

const TODOS = "TODOS";

export function FiltrosBusca({ filtros, onChange }: FiltrosBuscaProps) {
  return (
    <div className="flex flex-wrap items-center gap-3">
      <div className="flex items-center gap-1.5 text-sm text-muted-foreground">
        <Filter className="size-4" />
        Filtros:
      </div>

      <Select
        value={filtros.service ?? TODOS}
        onValueChange={(value) => onChange({ ...filtros, service: value === TODOS ? undefined : value })}
      >
        <SelectTrigger className="w-44">
          <SelectValue placeholder="Servico" />
        </SelectTrigger>
        <SelectContent>
          <SelectItem value={TODOS}>Todos os servicos</SelectItem>
          {SERVICOS_SIGLAS.map((sigla) => (
            <SelectItem key={sigla} value={sigla}>
              {SERVICO_LABEL[sigla]}
            </SelectItem>
          ))}
        </SelectContent>
      </Select>

      <Select
        value={filtros.status ?? TODOS}
        onValueChange={(value) => onChange({ ...filtros, status: value === TODOS ? undefined : value })}
      >
        <SelectTrigger className="w-44">
          <SelectValue placeholder="Status na fila" />
        </SelectTrigger>
        <SelectContent>
          <SelectItem value={TODOS}>Todos os status</SelectItem>
          {Object.values(StatusFila).map((status) => (
            <SelectItem key={status} value={status}>
              {STATUS_FILA_LABEL[status]}
            </SelectItem>
          ))}
        </SelectContent>
      </Select>

      <Select
        value={filtros.priority ?? TODOS}
        onValueChange={(value) => onChange({ ...filtros, priority: value === TODOS ? undefined : value })}
      >
        <SelectTrigger className="w-44">
          <SelectValue placeholder="Prioridade" />
        </SelectTrigger>
        <SelectContent>
          <SelectItem value={TODOS}>Todas as prioridades</SelectItem>
          {Object.values(PrioridadeFila).map((prioridade) => (
            <SelectItem key={prioridade} value={prioridade}>
              {PRIORIDADE_LABEL[prioridade]}
            </SelectItem>
          ))}
        </SelectContent>
      </Select>
    </div>
  );
}
