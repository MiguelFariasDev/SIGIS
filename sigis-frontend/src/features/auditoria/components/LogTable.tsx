import { AlertTriangle, ShieldCheck } from "lucide-react";
import { DataTable, type DataTableColumn } from "@/components/common/DataTable";
import { SecretariatChip } from "@/components/domain/SecretariatChip";
import type { LogAcesso } from "@/lib/types/logAcesso";
import { formatarDataHora } from "@/lib/utils/formatters";
import { cn } from "@/lib/utils";

interface LogTableProps {
  logs: LogAcesso[];
  /** T18 — destaca acessos cross-secretaria sem justificativa registrada. */
  destacarSemJustificativa?: boolean;
}

export function LogTable({ logs, destacarSemJustificativa }: LogTableProps) {
  const columns: DataTableColumn<LogAcesso>[] = [
    {
      key: "dataHora",
      header: "Data/hora",
      sortValue: (row) => row.dataHora,
      render: (row) => <span className="text-muted-foreground">{formatarDataHora(row.dataHora)}</span>,
    },
    {
      key: "profissional",
      header: "Profissional",
      sortValue: (row) => row.profissionalNome,
      render: (row) => <span className="font-medium text-foreground">{row.profissionalNome}</span>,
    },
    {
      key: "acao",
      header: "Acao",
      render: (row) => (
        <span className="inline-flex items-center gap-1.5">
          {row.crossUnidade && <ShieldCheck className="size-3.5 text-sus-blue" />}
          {row.acao}
        </span>
      ),
    },
    {
      key: "paciente",
      header: "Paciente",
      sortValue: (row) => row.personName,
      render: (row) => row.personName,
    },
    {
      key: "secretaria",
      header: "Secretaria",
      render: (row) => (row.secretariat ? <SecretariatChip secretariat={row.secretariat} /> : "-"),
    },
    {
      key: "baseLegal",
      header: "Base legal",
      className: "max-w-xs",
      render: (row) => <span className="text-xs text-muted-foreground">{row.baseLegal || "-"}</span>,
    },
    {
      key: "justificativa",
      header: "Justificativa",
      className: "max-w-xs",
      render: (row) => {
        const semJustificativa = row.crossUnidade && !row.justificativa;
        return (
          <span
            className={cn(
              "line-clamp-1 text-xs",
              semJustificativa && destacarSemJustificativa
                ? "flex items-center gap-1 font-medium text-sus-red"
                : "text-muted-foreground",
            )}
          >
            {semJustificativa && destacarSemJustificativa && <AlertTriangle className="size-3.5 shrink-0" />}
            {row.justificativa || (semJustificativa ? "Sem justificativa registrada" : "-")}
          </span>
        );
      },
    },
  ];

  return (
    <DataTable
      columns={columns}
      data={logs}
      rowKey={(row) => row.id}
      pageSize={15}
      emptyTitle="Nenhum log encontrado"
      emptyDescription="Ajuste os filtros para consultar o historico de acessos."
    />
  );
}
