import { AlertTriangle, Search } from "lucide-react";
import { Link } from "react-router-dom";
import { EmptyState } from "@/components/common/EmptyState";
import type { AlertaRecente } from "@/lib/types/indicadores";
import { formatarDistancia } from "@/lib/utils/formatters";

interface AlertasRecentesProps {
  alertas: AlertaRecente[];
}

export function AlertasRecentes({ alertas }: AlertasRecentesProps) {
  if (alertas.length === 0) {
    return <EmptyState title="Nenhum alerta recente" description="Nao ha duplicidades ou buscas ativas pendentes." />;
  }

  return (
    <ul className="space-y-2">
      {alertas.map((alerta) => (
        <li key={alerta.id}>
          <Link
            to={alerta.tipo === "DUPLICIDADE" ? `/duplicidades/${alerta.id}` : "/pacientes"}
            className="flex items-start gap-3 rounded-lg border border-border p-3 transition-colors hover:border-sus-blue/40 hover:bg-sus-blue-light/30"
          >
            <div
              className={
                alerta.tipo === "DUPLICIDADE"
                  ? "mt-0.5 flex size-8 shrink-0 items-center justify-center rounded-full bg-sus-yellow-light text-sus-yellow-dark"
                  : "mt-0.5 flex size-8 shrink-0 items-center justify-center rounded-full bg-sus-red-light text-sus-red"
              }
            >
              {alerta.tipo === "DUPLICIDADE" ? <AlertTriangle className="size-4" /> : <Search className="size-4" />}
            </div>
            <div className="min-w-0 flex-1">
              <p className="text-sm text-foreground">{alerta.descricao}</p>
              <p className="mt-0.5 text-xs text-muted-foreground">{formatarDistancia(alerta.dataHora)}</p>
            </div>
          </Link>
        </li>
      ))}
    </ul>
  );
}
