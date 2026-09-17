import { useQuery } from "@tanstack/react-query";
import { Download } from "lucide-react";
import { useState } from "react";
import { ErrorState } from "@/components/common/ErrorState";
import { LoadingState } from "@/components/common/LoadingState";
import { PageHeader } from "@/components/common/PageHeader";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import type { AuditoriaFiltros } from "@/lib/types/logAcesso";
import { exportarAuditoriaCsv, fetchAuditoria } from "./api";
import { LogTable } from "./components/LogTable";

export function AuditoriaPage() {
  const [filtros, setFiltros] = useState<AuditoriaFiltros>({});

  const auditoriaQuery = useQuery({
    queryKey: ["auditoria", filtros],
    queryFn: () => fetchAuditoria(filtros),
  });

  return (
    <div>
      <PageHeader
        title="Auditoria de acessos"
        description="Log de todo acesso a dados de paciente por profissional de unidade diferente da originadora (RF13)."
        actions={
          <Button variant="outline" onClick={() => exportarAuditoriaCsv()}>
            <Download className="size-4" />
            Exportar CSV
          </Button>
        }
      />

      <div className="mb-4 grid grid-cols-1 gap-3 sm:grid-cols-2 lg:grid-cols-4">
        <Input
          placeholder="Filtrar por acao..."
          value={filtros.acao ?? ""}
          onChange={(e) => setFiltros({ ...filtros, acao: e.target.value || undefined })}
        />
        <Input
          type="date"
          value={filtros.dataInicio ?? ""}
          onChange={(e) => setFiltros({ ...filtros, dataInicio: e.target.value || undefined })}
        />
        <Input
          type="date"
          value={filtros.dataFim ?? ""}
          onChange={(e) => setFiltros({ ...filtros, dataFim: e.target.value || undefined })}
        />
      </div>

      {auditoriaQuery.isLoading && <LoadingState />}
      {auditoriaQuery.isError && <ErrorState onRetry={() => auditoriaQuery.refetch()} />}
      {auditoriaQuery.data && <LogTable logs={auditoriaQuery.data} />}
    </div>
  );
}
