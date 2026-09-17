import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import type { PersonSearchFilters } from "@/lib/types/person";
import { buscarPessoas, criarPessoa, fetchPessoa, fetchTimeline } from "./api";

export function useBuscarPessoas(filtros: PersonSearchFilters) {
  return useQuery({
    queryKey: ["pessoas", "busca", filtros],
    queryFn: () => buscarPessoas(filtros),
  });
}

export function usePessoa(id: string | undefined) {
  return useQuery({
    queryKey: ["pessoas", id],
    queryFn: () => fetchPessoa(id!),
    enabled: !!id,
  });
}

export function useTimeline(id: string | undefined, secretariasLiberadas: string[] = []) {
  return useQuery({
    queryKey: ["pessoas", id, "timeline", secretariasLiberadas],
    queryFn: () => fetchTimeline(id!, secretariasLiberadas),
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
