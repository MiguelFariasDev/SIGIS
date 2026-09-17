import { useQuery } from "@tanstack/react-query";
import { AlertTriangle, Send, Users, UserX } from "lucide-react";
import { useState } from "react";
import { PageHeader } from "@/components/common/PageHeader";
import { IndicatorCard } from "@/components/domain/IndicatorCard";
import { Button } from "@/components/ui/button";
import { cn } from "@/lib/utils";
import type { PeriodoFiltro } from "@/lib/types/indicadores";
import { fetchAlertasRecentes, fetchAtendimentosPorDia, fetchFilaPorServico, fetchIndicadores } from "./api";
import { AlertasRecentes } from "./components/AlertasRecentes";
import { AtendimentosChart } from "./components/AtendimentosChart";
import { FilaPorServicoChart } from "./components/FilaPorServicoChart";

const PERIODOS: { value: PeriodoFiltro; label: string }[] = [
  { value: "HOJE", label: "Hoje" },
  { value: "7D", label: "7 dias" },
  { value: "30D", label: "30 dias" },
];

export function DashboardPage() {
  const [periodo, setPeriodo] = useState<PeriodoFiltro>("7D");

  const indicadoresQuery = useQuery({
    queryKey: ["indicadores", periodo],
    queryFn: () => fetchIndicadores(periodo),
  });

  const filaPorServicoQuery = useQuery({
    queryKey: ["indicadores", "fila-por-servico"],
    queryFn: fetchFilaPorServico,
  });

  const atendimentosPorDiaQuery = useQuery({
    queryKey: ["indicadores", "atendimentos-por-dia", periodo],
    queryFn: () => fetchAtendimentosPorDia(periodo),
  });

  const alertasQuery = useQuery({
    queryKey: ["indicadores", "alertas-recentes"],
    queryFn: fetchAlertasRecentes,
  });

  const indicadores = indicadoresQuery.data;

  return (
    <div>
      <PageHeader
        title="Painel de Indicadores"
        description="Visao consolidada da rede de cuidado a pessoas com TEA em Crateus/CE."
        actions={
          <div className="flex gap-1 rounded-lg border border-border bg-white p-1">
            {PERIODOS.map((item) => (
              <Button
                key={item.value}
                type="button"
                size="sm"
                variant="ghost"
                className={cn(periodo === item.value && "bg-sus-blue text-white hover:bg-sus-blue-dark hover:text-white")}
                onClick={() => setPeriodo(item.value)}
              >
                {item.label}
              </Button>
            ))}
          </div>
        }
      />

      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-4">
        <IndicatorCard
          icon={Users}
          label="Atendimentos no periodo"
          value={indicadores?.atendimentosNoPeriodo ?? 0}
          loading={indicadoresQuery.isLoading}
        />
        <IndicatorCard
          icon={UserX}
          label="Faltas no periodo"
          value={indicadores?.faltasNoPeriodo ?? 0}
          tone="danger"
          loading={indicadoresQuery.isLoading}
        />
        <IndicatorCard
          icon={Send}
          label="Encaminhamentos ativos"
          value={indicadores?.encaminhamentosAtivos ?? 0}
          loading={indicadoresQuery.isLoading}
        />
        <IndicatorCard
          icon={AlertTriangle}
          label="Duplicidades pendentes"
          value={indicadores?.duplicidadesPendentes ?? 0}
          tone={indicadores && indicadores.duplicidadesPendentes > 0 ? "danger" : "success"}
          loading={indicadoresQuery.isLoading}
        />
      </div>

      <div className="mt-6 grid grid-cols-1 gap-4 lg:grid-cols-2">
        <div className="rounded-xl border border-border bg-white p-5">
          <h2 className="mb-4 text-sm font-semibold text-foreground">Fila por servico</h2>
          {filaPorServicoQuery.data && <FilaPorServicoChart data={filaPorServicoQuery.data} />}
        </div>
        <div className="rounded-xl border border-border bg-white p-5">
          <h2 className="mb-4 text-sm font-semibold text-foreground">Atendimentos por dia</h2>
          {atendimentosPorDiaQuery.data && <AtendimentosChart data={atendimentosPorDiaQuery.data} />}
        </div>
      </div>

      <div className="mt-6 rounded-xl border border-border bg-white p-5">
        <h2 className="mb-4 text-sm font-semibold text-foreground">Alertas recentes</h2>
        {alertasQuery.data && <AlertasRecentes alertas={alertasQuery.data} />}
      </div>
    </div>
  );
}
