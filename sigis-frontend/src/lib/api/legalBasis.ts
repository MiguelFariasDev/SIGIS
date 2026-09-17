import { apiClient } from "./client";
import { ENDPOINTS } from "./endpoints";
import type { LegalBasis, Secretariat } from "@/lib/types/legalBasis";

interface BackendLegalBasis {
  secretariat: Secretariat;
  article: string;
  justification: string;
}

/**
 * Backend real (`GET /api/legal-basis`) devolve só `{secretariat, article,
 * justification}` — sem `id`/`code`/`description`/`purpose` (é uma tabela
 * estática de 3 linhas, uma por secretaria, sem persistência). `id`/`code`
 * são sintetizados a partir de `secretariat` (única por linha);
 * `description`/`purpose` reaproveitam `justification`.
 */
export async function fetchLegalBasis(): Promise<LegalBasis[]> {
  const { data } = await apiClient.get<BackendLegalBasis[]>(ENDPOINTS.legalBasis.base);
  return data.map((b) => ({
    id: b.secretariat,
    code: b.secretariat,
    description: b.justification,
    purpose: b.justification,
    article: b.article,
    secretariat: b.secretariat,
  }));
}
