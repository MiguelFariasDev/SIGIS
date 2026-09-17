import { useQuery } from "@tanstack/react-query";
import { useNavigate } from "react-router-dom";
import { DataTable, type DataTableColumn } from "@/components/common/DataTable";
import { ErrorState } from "@/components/common/ErrorState";
import { LoadingState } from "@/components/common/LoadingState";
import { StatusBadge } from "@/components/domain/StatusBadge";
import { fetchEncaminhamentosPorPessoa } from "@/features/encaminhamentos/api";
import type { Encaminhamento } from "@/lib/types/encaminhamento";
import { formatarDataHora } from "@/lib/utils/formatters";

interface AbaEncaminhamentosProps {
  pessoaId: string;
}

export function AbaEncaminhamentos({ pessoaId }: AbaEncaminhamentosProps) {
  const navigate = useNavigate();
  const encaminhamentosQuery = useQuery({
    queryKey: ["encaminhamentos", "pessoa", pessoaId],
    queryFn: () => fetchEncaminhamentosPorPessoa(pessoaId),
  });

  if (encaminhamentosQuery.isLoading) return <LoadingState />;
  if (encaminhamentosQuery.isError || !encaminhamentosQuery.data) {
    return <ErrorState onRetry={() => encaminhamentosQuery.refetch()} />;
  }

  const columns: DataTableColumn<Encaminhamento>[] = [
    {
      key: "trajeto",
      header: "Origem → Destino",
      render: (row) => (
        <span className="font-medium text-foreground">
          {row.unidadeOrigemSigla} → {row.unidadeDestinoSigla}
        </span>
      ),
    },
    {
      key: "motivo",
      header: "Motivo",
      className: "max-w-xs",
      render: (row) => <span className="line-clamp-1 text-muted-foreground">{row.motivo}</span>,
    },
    {
      key: "data",
      header: "Data",
      sortValue: (row) => row.dataEncaminhamento,
      render: (row) => <span className="text-muted-foreground">{formatarDataHora(row.dataEncaminhamento)}</span>,
    },
    {
      key: "status",
      header: "Status",
      render: (row) => <StatusBadge kind="encaminhamento" status={row.status} />,
    },
  ];

  return (
    <DataTable
      columns={columns}
      data={encaminhamentosQuery.data}
      rowKey={(row) => row.id}
      onRowClick={(row) => navigate(`/encaminhamentos/${row.id}/rastreio`)}
      emptyTitle="Nenhum encaminhamento registrado"
    />
  );
}
