import { apiClient } from "@/lib/api/client";
import { ENDPOINTS } from "@/lib/api/endpoints";
import { fetchUnidades } from "@/features/unidades/api";
import { mapPriorityFromApi, mapQueueStatusFromApi } from "@/lib/api/backendMappers";
import { StatusFila } from "@/lib/types/enums";
import type { FilaAtendimento } from "@/lib/types/fila";

interface BackendQueueEntry {
  id: string;
  personId: string;
  personName: string;
  unitId: string;
  specialty: string;
  priority: string;
  status: string;
  enteredAt: string;
  consecutiveAbsences: number;
}

function mapQueueEntryFromApi(e: BackendQueueEntry): FilaAtendimento {
  return {
    id: e.id,
    personId: e.personId,
    personName: e.personName,
    unidadeId: e.unitId,
    especialidade: e.specialty,
    prioridade: mapPriorityFromApi(e.priority),
    entradaFila: e.enteredAt,
    status: mapQueueStatusFromApi(e.status),
  };
}

/** `GET /api/unidades/{unidadeId}/fila` — rota real (não `/api/filas/unidade/{id}`). */
export async function fetchFilaPorUnidade(unidadeId: string, especialidade?: string): Promise<FilaAtendimento[]> {
  const { data } = await apiClient.get<BackendQueueEntry[]>(ENDPOINTS.unidades.fila(unidadeId), {
    params: { especialidade: especialidade || undefined },
  });
  return data.map(mapQueueEntryFromApi);
}

/**
 * Visao "rede inteira" (T07 nivel 1) — usada pelo COORDENADOR. Sem
 * equivalente no backend real (só existe fila por unidade) — agregamos
 * no cliente, buscando a fila de cada unidade conhecida.
 */
export async function fetchFilaRede(filtros: { unidadeId?: string; especialidade?: string } = {}): Promise<FilaAtendimento[]> {
  const unidades = await fetchUnidades();
  const alvo = filtros.unidadeId ? unidades.filter((u) => u.id === filtros.unidadeId) : unidades;
  const filas = await Promise.all(alvo.map((u) => fetchFilaPorUnidade(u.id, filtros.especialidade)));
  return filas.flat();
}

export async function fetchFilaPorPessoa(pessoaId: string): Promise<FilaAtendimento[]> {
  // Sem endpoint dedicado no backend real — agrega por unidade e filtra no cliente (gap: custoso em rede grande).
  const todas = await fetchFilaRede();
  return todas.filter((item) => item.personId === pessoaId);
}

/**
 * O backend real só suporta duas transições manuais: "chamar" (→ em
 * atendimento) e "comparecimento" (→ concluído/faltou). Não existe um PATCH
 * genérico de status nem uma transição de volta para "aguardando" — nesses
 * casos a chamada é rejeitada (gap documentado, mantém a UI de
 * drag-and-drop existente sem reescrevê-la).
 */
export async function atualizarStatusFila(id: string, status: StatusFila): Promise<FilaAtendimento> {
  if (status === StatusFila.EM_ATENDIMENTO) return chamar(id);
  if (status === StatusFila.CONCLUIDO) return registrarComparecimento(id, "COMPARECEU");
  if (status === StatusFila.FALTOU) return registrarComparecimento(id, "FALTOU");
  throw new Error(`Transição para "${status}" não é suportada pela API real.`);
}

export async function chamar(queueEntryId: string): Promise<FilaAtendimento> {
  const { data } = await apiClient.post<BackendQueueEntry>(ENDPOINTS.filas.chamar(queueEntryId));
  return mapQueueEntryFromApi(data);
}

export async function registrarComparecimento(
  queueEntryId: string,
  comparecimento: "COMPARECEU" | "FALTOU",
): Promise<FilaAtendimento> {
  const { data } = await apiClient.patch<BackendQueueEntry>(ENDPOINTS.filas.comparecimento(queueEntryId), {
    comparecimento,
  });
  return mapQueueEntryFromApi(data);
}

/**
 * "Chamar próximo" da unidade — o backend real chama uma entrada de fila
 * específica, não "a próxima da unidade". Buscamos a fila atual e chamamos
 * a primeira pessoa aguardando (maior prioridade primeiro).
 */
export async function chamarProximo(unidadeId: string): Promise<FilaAtendimento> {
  const fila = await fetchFilaPorUnidade(unidadeId);
  const ordemPrioridade: Record<string, number> = { URGENTE: 0, CURTO_PRAZO: 1, LISTA_ESPERA: 2 };
  const aguardando = fila
    .filter((item) => item.status === StatusFila.AGUARDANDO)
    .sort((a, b) => (ordemPrioridade[a.prioridade] ?? 9) - (ordemPrioridade[b.prioridade] ?? 9)
      || new Date(a.entradaFila).getTime() - new Date(b.entradaFila).getTime());

  const proximo = aguardando[0];
  if (!proximo) throw new Error("Não há pessoas aguardando nesta fila.");

  return chamar(proximo.id);
}
