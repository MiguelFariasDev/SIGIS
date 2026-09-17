import { useQuery } from "@tanstack/react-query";
import { UserRound } from "lucide-react";
import { useNavigate, useParams } from "react-router-dom";
import { ErrorState } from "@/components/common/ErrorState";
import { LoadingState } from "@/components/common/LoadingState";
import { PageHeader } from "@/components/common/PageHeader";
import { StatusBadge } from "@/components/domain/StatusBadge";
import { Button } from "@/components/ui/button";
import { fetchEncaminhamento } from "./api";
import { RastreioTimeline } from "./components/RastreioTimeline";

export function RastreioPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();

  const encaminhamentoQuery = useQuery({
    queryKey: ["encaminhamentos", id],
    queryFn: () => fetchEncaminhamento(id!),
    enabled: !!id,
  });

  if (encaminhamentoQuery.isLoading) return <LoadingState />;
  if (encaminhamentoQuery.isError || !encaminhamentoQuery.data) {
    return <ErrorState onRetry={() => encaminhamentoQuery.refetch()} />;
  }

  const encaminhamento = encaminhamentoQuery.data;

  return (
    <div className="mx-auto max-w-3xl">
      <PageHeader
        title={`Rastreio — ${encaminhamento.personName}`}
        description={`${encaminhamento.unidadeOrigemSigla} → ${encaminhamento.unidadeDestinoSigla}`}
        actions={<StatusBadge kind="encaminhamento" status={encaminhamento.status} />}
      />

      <RastreioTimeline encaminhamento={encaminhamento} />

      <div className="mt-4 rounded-xl border border-border bg-white p-5">
        <p className="text-sm font-semibold text-foreground">Motivo</p>
        <p className="mt-1 text-sm text-muted-foreground">{encaminhamento.motivo}</p>
      </div>

      <div className="mt-4 flex justify-end">
        <Button variant="outline" onClick={() => navigate(`/pacientes/${encaminhamento.personId}`)}>
          <UserRound className="size-4" />
          Ver ficha do paciente
        </Button>
      </div>
    </div>
  );
}
