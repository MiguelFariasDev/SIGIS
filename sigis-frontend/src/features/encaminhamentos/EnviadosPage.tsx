import { useQuery } from "@tanstack/react-query";
import { useNavigate } from "react-router-dom";
import { DataTable, type DataTableColumn } from "@/components/common/DataTable";
import { ErrorState } from "@/components/common/ErrorState";
import { LoadingState } from "@/components/common/LoadingState";
import { PageHeader } from "@/components/common/PageHeader";
import { StatusBadge } from "@/components/domain/StatusBadge";
import type { Encaminhamento } from "@/lib/types/encaminhamento";
import { formatarDataHora } from "@/lib/utils/formatters";
import { useAuthStore } from "@/stores/authStore";
import { fetchEncaminhamentosEnviados } from "./api";

/** Encaminhamentos enviados pela unidade do profissional logado. */
export function EnviadosPage() {
  const usuario = useAuthStore((s) => s.usuario);
  const navigate = useNavigate();

  const enviadosQuery = useQuery({
    queryKey: ["encaminhamentos", "enviados", usuario?.unidadeId],
    queryFn: () => fetchEncaminhamentosEnviados(usuario!.unidadeId),
    enabled: !!usuario,
  });

  const columns: DataTableColumn<Encaminhamento>[] = [
    {
      key: "paciente",
      header: "Paciente",
      render: (row) => <span className="font-medium text-foreground">{row.personName}</span>,
    },
    {
      key: "trajeto",
      header: "Destino",
      render: (row) => `${row.unidadeOrigemSigla} → ${row.unidadeDestinoSigla}`,
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
    <div>
      <PageHeader title="Encaminhamentos enviados" description="Encaminhamentos enviados pela sua unidade para a rede." />

      {enviadosQuery.isLoading && <LoadingState />}
      {enviadosQuery.isError && <ErrorState onRetry={() => enviadosQuery.refetch()} />}
      {enviadosQuery.data && (
        <DataTable
          columns={columns}
          data={enviadosQuery.data}
          rowKey={(row) => row.id}
          onRowClick={(row) => navigate(`/encaminhamentos/${row.id}/rastreio`)}
          emptyTitle="Nenhum encaminhamento enviado"
          emptyDescription="Sua unidade ainda nao enviou encaminhamentos."
        />
      )}
    </div>
  );
}
