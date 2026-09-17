import { useQuery } from "@tanstack/react-query";
import { Download, ShieldCheck } from "lucide-react";
import { useMemo, useState } from "react";
import { EmptyState } from "@/components/common/EmptyState";
import { ErrorState } from "@/components/common/ErrorState";
import { LoadingState } from "@/components/common/LoadingState";
import { PageHeader } from "@/components/common/PageHeader";
import { ConsentCard } from "@/components/domain/ConsentCard";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import type { ConsentimentosFiltros, ConsentType } from "@/lib/types/personConsent";
import { fetchConsentimentosGlobal } from "./api";

const TIPO_LABEL: Record<ConsentType, string> = {
  Clinical: "Clinico (Saude)",
  Educational: "Educacional",
  SocialAssistance: "Assistencia Social",
  Research: "Pesquisa",
};

function exportarCsv(itens: { personName: string; type: string; granted: boolean; grantedAt: string; revokedAt?: string; version: string }[]) {
  const cabecalho = "pessoa,tipo,status,concedido_em,revogado_em,versao";
  const linhas = itens.map((item) =>
    [item.personName, item.type, item.granted ? "concedido" : "revogado", item.grantedAt, item.revokedAt ?? "", item.version]
      .map((campo) => `"${String(campo).replace(/"/g, '""')}"`)
      .join(","),
  );
  const csv = [cabecalho, ...linhas].join("\n");
  const blob = new Blob([csv], { type: "text/csv;charset=utf-8" });
  const url = window.URL.createObjectURL(blob);
  const link = document.createElement("a");
  link.href = url;
  link.setAttribute("download", "consentimentos-sigis.csv");
  document.body.appendChild(link);
  link.click();
  link.remove();
  window.URL.revokeObjectURL(url);
}

/** T17 — painel de consentimentos LGPD (visao DPO/COORDENADOR). */
export function ConsentimentosPage() {
  const [filtros, setFiltros] = useState<ConsentimentosFiltros>({});
  const [busca, setBusca] = useState("");

  const consentimentosQuery = useQuery({
    queryKey: ["consentimentos", filtros],
    queryFn: () => fetchConsentimentosGlobal(filtros),
  });

  const itensFiltrados = useMemo(() => {
    const termo = busca.trim().toLowerCase();
    return (consentimentosQuery.data ?? []).filter(
      (item) => termo === "" || item.personName.toLowerCase().includes(termo),
    );
  }, [consentimentosQuery.data, busca]);

  return (
    <div>
      <PageHeader
        title="Consentimentos LGPD"
        description="Consentimentos concedidos e revogados por pessoa, tipo e periodo (visao DPO)."
        actions={
          <Button variant="outline" onClick={() => exportarCsv(itensFiltrados)} disabled={itensFiltrados.length === 0}>
            <Download className="size-4" />
            Exportar CSV
          </Button>
        }
      />

      <div className="mb-4 grid grid-cols-1 gap-3 sm:grid-cols-2 lg:grid-cols-4">
        <Input placeholder="Filtrar por pessoa..." value={busca} onChange={(e) => setBusca(e.target.value)} />
        <Select
          value={filtros.type ?? "TODOS"}
          onValueChange={(v) => setFiltros((f) => ({ ...f, type: v === "TODOS" ? undefined : (v as ConsentType) }))}
        >
          <SelectTrigger className="w-full">
            <SelectValue placeholder="Tipo" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="TODOS">Todos os tipos</SelectItem>
            {(Object.keys(TIPO_LABEL) as ConsentType[]).map((tipo) => (
              <SelectItem key={tipo} value={tipo}>
                {TIPO_LABEL[tipo]}
              </SelectItem>
            ))}
          </SelectContent>
        </Select>
        <Select
          value={filtros.status ?? "TODOS"}
          onValueChange={(v) =>
            setFiltros((f) => ({ ...f, status: v === "TODOS" ? undefined : (v as "granted" | "revoked") }))
          }
        >
          <SelectTrigger className="w-full">
            <SelectValue placeholder="Status" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="TODOS">Todos os status</SelectItem>
            <SelectItem value="granted">Concedido</SelectItem>
            <SelectItem value="revoked">Revogado</SelectItem>
          </SelectContent>
        </Select>
        <Input
          type="date"
          value={filtros.dataInicio ?? ""}
          onChange={(e) => setFiltros((f) => ({ ...f, dataInicio: e.target.value || undefined }))}
        />
      </div>

      {consentimentosQuery.isLoading && <LoadingState />}
      {consentimentosQuery.isError && <ErrorState onRetry={() => consentimentosQuery.refetch()} />}
      {consentimentosQuery.data && itensFiltrados.length === 0 && (
        <EmptyState icon={ShieldCheck} title="Nenhum consentimento encontrado" description="Ajuste os filtros para consultar os consentimentos registrados." />
      )}

      {itensFiltrados.length > 0 && (
        <div className="space-y-2">
          {itensFiltrados.map((item) => (
            <div key={item.id}>
              <p className="mb-1 text-xs font-medium text-muted-foreground">{item.personName}</p>
              <ConsentCard consent={item} />
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
