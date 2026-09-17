import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import type { PersonSearchFilters } from "@/lib/types/person";
import { buscarPessoas, criarPessoa, fetchPessoa, fetchTimeline } from "./api";

/** `GET /api/pessoas/busca` real exige `termo` não vazio — sem ele, o backend responde 400 em vez de listar tudo. */
export function useBuscarPessoas(filtros: PersonSearchFilters) {
  return useQuery({
    queryKey: ["pessoas", "busca", filtros],
    queryFn: () => buscarPessoas(filtros),
    enabled: !!filtros.term?.trim(),
  });
}

export function usePessoa(id: string | undefined) {
  return useQuery({
    queryKey: ["pessoas", id],
    queryFn: () => fetchPessoa(id!),
    enabled: !!id,
  });
}

export function useTimeline(id: string | undefined, opts: { completo?: boolean; justificativa?: string } = {}) {
  return useQuery({
    queryKey: ["pessoas", id, "timeline", opts.completo ?? false],
    queryFn: () => fetchTimeline(id!, opts),
    enabled: !!id,
  });
}

export function useCriarPessoa() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: criarPessoa,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["pessoas"] });
    },
  });
}
