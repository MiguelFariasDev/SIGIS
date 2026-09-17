import { apiClient } from "./client";
import { ENDPOINTS } from "./endpoints";
import type { LegalBasis } from "@/lib/types/legalBasis";

export async function fetchLegalBasis(): Promise<LegalBasis[]> {
  const { data } = await apiClient.get<LegalBasis[]>(ENDPOINTS.legalBasis.base);
  return data;
}
