import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { HeartPulse, Plus, Trash2 } from "lucide-react";
import { useState } from "react";
import { toast } from "sonner";
import { EmptyState } from "@/components/common/EmptyState";
import { ErrorState } from "@/components/common/ErrorState";
import { LoadingState } from "@/components/common/LoadingState";
import { Button } from "@/components/ui/button";
import { excluirTratamentoConcomitante, fetchTratamentosConcomitantes } from "../api";
import { AddConcurrentTreatmentModal } from "./AddConcurrentTreatmentModal";

interface AbaTratamentosConcomitantesProps {
  pessoaId: string;
}

const DIA_LABEL: Record<string, string> = {
  Monday: "Segunda-feira",
  Tuesday: "Terca-feira",
  Wednesday: "Quarta-feira",
  Thursday: "Quinta-feira",
  Friday: "Sexta-feira",
  Saturday: "Sabado",
  Sunday: "Domingo",
};

export function AbaTratamentosConcomitantes({ pessoaId }: AbaTratamentosConcomitantesProps) {
  const [modalAberto, setModalAberto] = useState(false);
  const queryClient = useQueryClient();

  const tratamentosQuery = useQuery({
    queryKey: ["pessoas", pessoaId, "tratamentos-concomitantes"],
    queryFn: () => fetchTratamentosConcomitantes(pessoaId),
  });

  const excluirMutation = useMutation({
    mutationFn: (treatmentId: string) => excluirTratamentoConcomitante(pessoaId, treatmentId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["pessoas", pessoaId, "tratamentos-concomitantes"] });
      toast.success("Tratamento removido.");
    },
  });

  if (tratamentosQuery.isLoading) return <LoadingState />;
  if (tratamentosQuery.isError) return <ErrorState onRetry={() => tratamentosQuery.refetch()} />;

  return (
    <div>
      <div className="mb-3 flex justify-end">
        <Button variant="outline" onClick={() => setModalAberto(true)}>
          <Plus className="size-4" />
          Adicionar tratamento
        </Button>
      </div>

      {tratamentosQuery.data && tratamentosQuery.data.length === 0 && (
        <EmptyState
          icon={HeartPulse}
          title="Nenhum tratamento concomitante"
          description="Nenhum atendimento fora da rede foi registrado para esta pessoa."
        />
      )}

      {tratamentosQuery.data && tratamentosQuery.data.length > 0 && (
        <div className="space-y-2">
          {tratamentosQuery.data.map((tratamento) => (
            <div key={tratamento.id} className="flex items-start justify-between gap-3 rounded-xl border border-border bg-white p-4">
              <div>
                <p className="text-sm font-medium text-foreground">
                  {tratamento.specialty} — {tratamento.professionalName}
                </p>
                <p className="mt-0.5 text-xs text-muted-foreground">
                  {tratamento.location} — {DIA_LABEL[tratamento.dayOfWeek] ?? tratamento.dayOfWeek} {tratamento.startTime}–
                  {tratamento.endTime}
                </p>
                {tratamento.notes && <p className="mt-1 text-xs text-muted-foreground/80">{tratamento.notes}</p>}
              </div>
              <Button
                size="sm"
                variant="outline"
                className="shrink-0 text-sus-red hover:text-sus-red"
                onClick={() => excluirMutation.mutate(tratamento.id)}
                disabled={excluirMutation.isPending}
              >
                <Trash2 className="size-3.5" />
              </Button>
            </div>
          ))}
        </div>
      )}

      <AddConcurrentTreatmentModal open={modalAberto} onOpenChange={setModalAberto} personId={pessoaId} />
    </div>
  );
}
