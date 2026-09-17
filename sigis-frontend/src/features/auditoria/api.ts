import { apiClient } from "@/lib/api/client";
import { ENDPOINTS } from "@/lib/api/endpoints";
import type { AuditoriaFiltros, LogAcesso } from "@/lib/types/logAcesso";

export async function fetchAuditoria(filtros: AuditoriaFiltros): Promise<LogAcesso[]> {
  const { data } = await apiClient.get<LogAcesso[]>(ENDPOINTS.auditoria.base, {
    params: {
      personId: filtros.personId || undefined,
      profissionalId: filtros.profissionalId || undefined,
      acao: filtros.acao || undefined,
      dataInicio: filtros.dataInicio || undefined,
      dataFim: filtros.dataFim || undefined,
      secretariat: filtros.secretariat || undefined,
      crossUnidade: filtros.crossUnidadeApenas || undefined,
    },
  });
  return data;
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
