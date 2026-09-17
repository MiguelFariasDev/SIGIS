import { useNavigate } from "react-router-dom";
import { DataTable, type DataTableColumn } from "@/components/common/DataTable";
import { EmptyState } from "@/components/common/EmptyState";
import { PriorityBadge } from "@/components/domain/PriorityBadge";
import { ServiceChip } from "@/components/domain/ServiceChip";
import { StatusBadge } from "@/components/domain/StatusBadge";
import type { PersonSearchResult } from "@/lib/types/person";
import type { PrioridadeFila, StatusFila } from "@/lib/types/enums";
import { formatarData, formatarIdade } from "@/lib/utils/formatters";
import { Search } from "lucide-react";

interface ResultadoBuscaProps {
  resultados: PersonSearchResult[];
}

export function ResultadoBusca({ resultados }: ResultadoBuscaProps) {
  const navigate = useNavigate();

  if (resultados.length === 0) {
    return (
      <EmptyState
        icon={Search}
        title="Nenhum paciente encontrado"
        description="Ajuste os termos de busca ou os filtros, ou cadastre um novo paciente."
      />
    );
  }

  const columns: DataTableColumn<PersonSearchResult>[] = [
    {
      key: "nome",
      header: "Nome",
      sortValue: (row) => row.fullName,
      render: (row) => <span className="font-medium text-foreground">{row.fullName}</span>,
    },
    {
      key: "dataNascimento",
      header: "Data nasc.",
      sortValue: (row) => row.birthDate,
      render: (row) => (
        <span className="text-muted-foreground">
          {formatarData(row.birthDate)} ({formatarIdade(row.birthDate)})
        </span>
      ),
    },
    {
      key: "servicos",
      header: "Servicos",
      render: (row) => (
        <div className="flex flex-wrap gap-1">
          {(row.services ?? []).map((servico) => (
            <ServiceChip key={servico} sigla={servico} />
          ))}
        </div>
      ),
    },
    {
      key: "status",
      header: "Status",
      render: (row) => (row.queueStatus ? <StatusBadge kind="fila" status={row.queueStatus as StatusFila} /> : "-"),
    },
    {
      key: "prioridade",
      header: "Prioridade",
      render: (row) => (row.priority ? <PriorityBadge prioridade={row.priority as PrioridadeFila} /> : "-"),
    },
  ];

  return (
    <DataTable
      columns={columns}
      data={resultados}
      rowKey={(row) => row.id}
      onRowClick={(row) => navigate(`/pacientes/${row.id}`)}
    />
  );
}
