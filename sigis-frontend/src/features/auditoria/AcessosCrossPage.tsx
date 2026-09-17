import { useQuery } from "@tanstack/react-query";
import { Download } from "lucide-react";
import { useState } from "react";
import { ErrorState } from "@/components/common/ErrorState";
import { LoadingState } from "@/components/common/LoadingState";
import { PageHeader } from "@/components/common/PageHeader";
import { Button } from "@/components/ui/button";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { profissionaisMock } from "@/lib/mocks/data/profissionais.mock";
import type { AuditoriaFiltros } from "@/lib/types/logAcesso";
import type { Secretariat } from "@/lib/types/legalBasis";
import { SECRETARIAT_LABEL } from "@/lib/utils/constants";
import { exportarAuditoriaCsv, fetchAuditoria } from "./api";
import { LogTable } from "./components/LogTable";

const SECRETARIATS: Secretariat[] = ["Health", "Education", "SocialAssistance"];

/** T18 — auditoria de acessos cross-secretaria (visao DPO/AUDITOR — RF12/RF13). */
export function AcessosCrossPage() {
  const [filtros, setFiltros] = useState<AuditoriaFiltros>({ crossUnidadeApenas: true });

  const auditoriaQuery = useQuery({
    queryKey: ["auditoria", "cross", filtros],
    queryFn: () => fetchAuditoria(filtros),
  });

  return (
    <div>
      <PageHeader
        title="Acessos cross-secretaria"
        description="Todo acesso a conteudo de outra secretaria, com base legal e justificativa (RF12/RF13)."
        actions={
          <Button variant="outline" onClick={() => exportarAuditoriaCsv()}>
            <Download className="size-4" />
            Exportar CSV
          </Button>
        }
      />

      <div className="mb-4 grid grid-cols-1 gap-3 sm:grid-cols-2 lg:grid-cols-3">
        <Select
          value={filtros.profissionalId ?? "TODOS"}
          onValueChange={(v) => setFiltros((f) => ({ ...f, profissionalId: v === "TODOS" ? undefined : v }))}
        >
          <SelectTrigger className="w-full">
            <SelectValue placeholder="Profissional" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="TODOS">Todos os profissionais</SelectItem>
            {profissionaisMock.map((prof) => (
              <SelectItem key={prof.id} value={prof.id}>
                {prof.nome}
              </SelectItem>
            ))}
          </SelectContent>
        </Select>

        <Select
          value={filtros.secretariat ?? "TODAS"}
          onValueChange={(v) => setFiltros((f) => ({ ...f, secretariat: v === "TODAS" ? undefined : (v as Secretariat) }))}
        >
          <SelectTrigger className="w-full">
            <SelectValue placeholder="Secretaria" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="TODAS">Todas as secretarias</SelectItem>
            {SECRETARIATS.map((s) => (
              <SelectItem key={s} value={s}>
                {SECRETARIAT_LABEL[s]}
              </SelectItem>
            ))}
          </SelectContent>
        </Select>
      </div>

      {auditoriaQuery.isLoading && <LoadingState />}
      {auditoriaQuery.isError && <ErrorState onRetry={() => auditoriaQuery.refetch()} />}
      {auditoriaQuery.data && <LogTable logs={auditoriaQuery.data} destacarSemJustificativa />}
    </div>
  );
}
