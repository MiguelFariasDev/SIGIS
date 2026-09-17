import { useQuery } from "@tanstack/react-query";
import { DataTable, type DataTableColumn } from "@/components/common/DataTable";
import { ErrorState } from "@/components/common/ErrorState";
import { LoadingState } from "@/components/common/LoadingState";
import { PriorityBadge } from "@/components/domain/PriorityBadge";
import { StatusBadge } from "@/components/domain/StatusBadge";
import { fetchFilaPorPessoa } from "@/features/filas/api";
import { unidadesMock } from "@/lib/mocks/data/unidades.mock";
import type { FilaAtendimento } from "@/lib/types/fila";
import { formatarDistancia } from "@/lib/utils/formatters";

interface AbaFilaProps {
  pessoaId: string;
}

export function AbaFila({ pessoaId }: AbaFilaProps) {
  const filaQuery = useQuery({
    queryKey: ["filas", "pessoa", pessoaId],
    queryFn: () => fetchFilaPorPessoa(pessoaId),
  });

  if (filaQuery.isLoading) return <LoadingState />;
  if (filaQuery.isError || !filaQuery.data) return <ErrorState onRetry={() => filaQuery.refetch()} />;

  const columns: DataTableColumn<FilaAtendimento>[] = [
    {
      key: "unidade",
      header: "Unidade",
      render: (row) => unidadesMock.find((u) => u.id === row.unidadeId)?.nome ?? row.unidadeId,
    },
    {
      key: "especialidade",
      header: "Especialidade",
      render: (row) => row.especialidade,
    },
    {
      key: "prioridade",
      header: "Prioridade",
      render: (row) => <PriorityBadge prioridade={row.prioridade} />,
    },
    {
      key: "status",
      header: "Status",
      render: (row) => <StatusBadge kind="fila" status={row.status} />,
    },
    {
      key: "entradaFila",
      header: "Na fila desde",
      sortValue: (row) => row.entradaFila,
      render: (row) => <span className="text-muted-foreground">{formatarDistancia(row.entradaFila)}</span>,
    },
  ];

  return (
    <DataTable
      columns={columns}
      data={filaQuery.data}
      rowKey={(row) => row.id}
      emptyTitle="Nao ha posicoes em fila"
      emptyDescription="Este paciente nao esta em nenhuma fila de atendimento no momento."
    />
  );
}
