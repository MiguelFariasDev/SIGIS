import { apiClient } from "@/lib/api/client";
import { ENDPOINTS } from "@/lib/api/endpoints";
import type { AuditoriaFiltros, LogAcesso } from "@/lib/types/logAcesso";

interface BackendAccessLog {
  id: string;
  personId: string;
  personName: string;
  professionalId: string;
  professionalName: string;
  action: string;
  legalBasis: string;
  justification?: string;
  dateTime: string;
  isCrossUnit: boolean;
}

function mapAccessLog(log: BackendAccessLog): LogAcesso {
  return {
    id: log.id,
    personId: log.personId,
    personName: log.personName,
    profissionalId: log.professionalId,
    profissionalNome: log.professionalName,
    acao: log.action,
    baseLegal: log.legalBasis,
    justificativa: log.justification,
    dataHora: log.dateTime,
    crossUnidade: log.isCrossUnit,
  };
}

/**
 * `GET /api/auditoria/acessos` real não aceita `acao`/`secretariat`/
 * `crossUnidade` como filtros (só pessoa, profissional e período) — os
 * demais filtros do formulário são aplicados no cliente (gap documentado).
 */
export async function fetchAuditoria(filtros: AuditoriaFiltros): Promise<LogAcesso[]> {
  const { data } = await apiClient.get<BackendAccessLog[]>(ENDPOINTS.auditoria.acessos, {
    params: {
      pessoaId: filtros.personId || undefined,
      profissionalId: filtros.profissionalId || undefined,
      dataInicio: filtros.dataInicio || undefined,
      dataFim: filtros.dataFim || undefined,
    },
  });

  let logs = data.map(mapAccessLog);
  if (filtros.acao) logs = logs.filter((l) => l.acao === filtros.acao);
  if (filtros.crossUnidadeApenas) logs = logs.filter((l) => l.crossUnidade);
  return logs;
}

export async function exportarAuditoriaCsv(): Promise<void> {
  const { data } = await apiClient.get(ENDPOINTS.auditoria.exportarCsv, { responseType: "blob" });
  const url = window.URL.createObjectURL(new Blob([data]));
  const link = document.createElement("a");
  link.href = url;
  link.setAttribute("download", "auditoria-sigis.csv");
  document.body.appendChild(link);
  link.click();
  link.remove();
  window.URL.revokeObjectURL(url);
}
