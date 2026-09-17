import { useQuery } from "@tanstack/react-query";
import { AlertTriangle, Search } from "lucide-react";
import { useNavigate } from "react-router-dom";
import { DataTable, type DataTableColumn } from "@/components/common/DataTable";
import { ErrorState } from "@/components/common/ErrorState";
import { LoadingState } from "@/components/common/LoadingState";
import { PageHeader } from "@/components/common/PageHeader";
import { StatusBadge } from "@/components/domain/StatusBadge";
import { Button } from "@/components/ui/button";
import type { AlertaDuplicidade } from "@/lib/types/duplicidade";
import { formatarDistancia, formatarPercentual } from "@/lib/utils/formatters";
import { fetchDuplicidades } from "./api";

export function RevisaoPage() {
  const navigate = useNavigate();
  const duplicidadesQuery = useQuery({
    queryKey: ["duplicidades"],
    queryFn: () => fetchDuplicidades(),
  });

  const columns: DataTableColumn<AlertaDuplicidade>[] = [
    {
      key: "pessoaA",
      header: "Pessoa A",
      render: (row) => <span className="font-medium text-foreground">{row.person1?.fullName ?? "-"}</span>,
    },
    {
      key: "pessoaB",
      header: "Pessoa B",
      render: (row) => <span className="font-medium text-foreground">{row.person2?.fullName ?? "-"}</span>,
    },
    {
      key: "score",
      header: "Score",
      sortValue: (row) => row.scoreSimilaridade,
      render: (row) => (
        <span className="font-medium text-sus-yellow-dark">{formatarPercentual(row.scoreSimilaridade)}</span>
      ),
    },
    {
      key: "status",
      header: "Status",
      render: (row) => <StatusBadge kind="duplicidade" status={row.status} />,
    },
    {
      key: "criadoEm",
      header: "Detectado",
      sortValue: (row) => row.criadoEm,
      render: (row) => <span className="text-muted-foreground">{formatarDistancia(row.criadoEm)}</span>,
    },
    {
      key: "acoes",
      header: "",
      render: (row) => (
        <Button size="sm" variant="outline" onClick={() => navigate(`/duplicidades/${row.id}`)}>
          <Search className="size-3.5" />
          Revisar
        </Button>
      ),
    },
  ];

  return (
    <div>
      <PageHeader
        title="Fila de duplicidades"
        description="Pares candidatos a mesma pessoa, detectados por CNS, CPF ou similaridade de nome."
      />

      {duplicidadesQuery.isLoading && <LoadingState />}
      {duplicidadesQuery.isError && <ErrorState onRetry={() => duplicidadesQuery.refetch()} />}
      {duplicidadesQuery.data && (
        <DataTable
          columns={columns}
          data={duplicidadesQuery.data}
          rowKey={(row) => row.id}
          emptyTitle="Nenhuma duplicidade pendente"
          emptyDescription="Nao ha pares candidatos aguardando revisao no momento."
        />
      )}

      {duplicidadesQuery.data && duplicidadesQuery.data.length > 0 && (
        <p className="mt-3 flex items-center gap-1.5 text-xs text-muted-foreground">
          <AlertTriangle className="size-3.5 text-sus-yellow-dark" />
          Nenhum cadastro e mesclado automaticamente — toda decisao exige revisao humana.
        </p>
      )}
    </div>
  );
}
