import { useQuery } from "@tanstack/react-query";
import { useNavigate } from "react-router-dom";
import { DataTable, type DataTableColumn } from "@/components/common/DataTable";
import { ErrorState } from "@/components/common/ErrorState";
import { LoadingState } from "@/components/common/LoadingState";
import { ServiceChip } from "@/components/domain/ServiceChip";
import { StatusBadge } from "@/components/domain/StatusBadge";
import { fetchAtendimentosPorPessoa } from "@/features/atendimentos/api";
import type { Attendance } from "@/lib/types/attendance";
import { TIPO_SESSAO_LABEL } from "@/lib/utils/constants";
import { formatarDataHora } from "@/lib/utils/formatters";

interface AbaAtendimentosProps {
  pessoaId: string;
}

export function AbaAtendimentos({ pessoaId }: AbaAtendimentosProps) {
  const navigate = useNavigate();
  const atendimentosQuery = useQuery({
    queryKey: ["atendimentos", "pessoa", pessoaId],
    queryFn: () => fetchAtendimentosPorPessoa(pessoaId),
  });

  if (atendimentosQuery.isLoading) return <LoadingState />;
  if (atendimentosQuery.isError || !atendimentosQuery.data) {
    return <ErrorState onRetry={() => atendimentosQuery.refetch()} />;
  }

  const columns: DataTableColumn<Attendance>[] = [
    {
      key: "unidade",
      header: "Unidade",
      render: (row) => <ServiceChip sigla={row.unitCode} />,
    },
    {
      key: "tipoSessao",
      header: "Sessao",
      render: (row) => (
        <span>
          {TIPO_SESSAO_LABEL[row.sessionType]} <span className="text-muted-foreground">#{row.sessionNumber}</span>
        </span>
      ),
    },
    {
      key: "profissional",
      header: "Profissional",
      render: (row) => row.professionalName,
    },
    {
      key: "dataHora",
      header: "Data",
      sortValue: (row) => row.dateTime,
      render: (row) => <span className="text-muted-foreground">{formatarDataHora(row.dateTime)}</span>,
    },
    {
      key: "comparecimento",
      header: "Comparecimento",
      render: (row) => <StatusBadge kind="comparecimento" status={row.attendanceStatus} />,
    },
  ];

  return (
    <DataTable
      columns={columns}
      data={atendimentosQuery.data}
      rowKey={(row) => row.id}
      onRowClick={(row) => navigate(`/atendimentos/${row.id}`)}
      emptyTitle="Nenhum atendimento registrado"
    />
  );
}
