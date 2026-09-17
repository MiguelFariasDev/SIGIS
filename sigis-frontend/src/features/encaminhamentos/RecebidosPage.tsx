import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Check, Eye, Inbox, X } from "lucide-react";
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { toast } from "sonner";
import { EmptyState } from "@/components/common/EmptyState";
import { ErrorState } from "@/components/common/ErrorState";
import { LoadingState } from "@/components/common/LoadingState";
import { PageHeader } from "@/components/common/PageHeader";
import { PriorityBadge } from "@/components/domain/PriorityBadge";
import { ServiceChip } from "@/components/domain/ServiceChip";
import { Button } from "@/components/ui/button";
import { Dialog, DialogContent, DialogFooter, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Field, FieldLabel } from "@/components/ui/field";
import { Textarea } from "@/components/ui/textarea";
import { formatarDataHora } from "@/lib/utils/formatters";
import { useAuthStore } from "@/stores/authStore";
import { aceitarEncaminhamento, fetchEncaminhamentosRecebidos, recusarEncaminhamento } from "./api";

/**
 * T13 — Encaminhamentos recebidos pela unidade do profissional logado
 * (Fluxo 4: Sidebar → Recebidos → Aceitar cria QueueEntry / Recusar exige motivo).
 */
export function RecebidosPage() {
  const usuario = useAuthStore((s) => s.usuario);
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const [encaminhamentoRecusando, setEncaminhamentoRecusando] = useState<string | null>(null);
  const [motivoRecusa, setMotivoRecusa] = useState("");

  const recebidosQuery = useQuery({
    queryKey: ["encaminhamentos", "recebidos", usuario?.unidadeId],
    queryFn: () => fetchEncaminhamentosRecebidos(),
    enabled: !!usuario,
  });

  const aceitarMutation = useMutation({
    mutationFn: (id: string) => aceitarEncaminhamento(id),
    onSuccess: (encaminhamento) => {
      queryClient.invalidateQueries({ queryKey: ["encaminhamentos", "recebidos"] });
      queryClient.invalidateQueries({ queryKey: ["filas"] });
      toast.success("Encaminhamento aceito. Paciente incluido na fila.");
      navigate(`/filas/${encaminhamento.unidadeDestinoId}`);
    },
  });

  const recusarMutation = useMutation({
    mutationFn: ({ id, motivo }: { id: string; motivo: string }) => recusarEncaminhamento(id, { motivo }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["encaminhamentos", "recebidos"] });
      toast.success("Encaminhamento recusado.");
      setEncaminhamentoRecusando(null);
      setMotivoRecusa("");
    },
    onError: () => toast.error("Nao foi possivel recusar o encaminhamento."),
  });

  return (
    <div>
      <PageHeader
        title="Encaminhamentos recebidos"
        description="Encaminhamentos pendentes de decisao para a sua unidade."
      />

      {recebidosQuery.isLoading && <LoadingState />}
      {recebidosQuery.isError && <ErrorState onRetry={() => recebidosQuery.refetch()} />}

      {recebidosQuery.data && recebidosQuery.data.length === 0 && (
        <EmptyState icon={Inbox} title="Nenhum encaminhamento pendente" description="Nao ha encaminhamentos aguardando decisao para a sua unidade." />
      )}

      {recebidosQuery.data && recebidosQuery.data.length > 0 && (
        <div className="space-y-3">
          {recebidosQuery.data.map((encaminhamento) => (
            <div key={encaminhamento.id} className="rounded-xl border border-border bg-white p-4">
              <div className="flex flex-wrap items-start justify-between gap-3">
                <div>
                  <p className="text-sm font-semibold text-foreground">{encaminhamento.personName}</p>
                  <p className="mt-0.5 flex items-center gap-1.5 text-xs text-muted-foreground">
                    <ServiceChip sigla={encaminhamento.unidadeOrigemSigla} /> → {encaminhamento.unidadeDestinoSigla}
                    <span>— {formatarDataHora(encaminhamento.dataEncaminhamento)}</span>
                  </p>
                </div>
                <PriorityBadge prioridade={encaminhamento.prioridade} />
              </div>

              <p className="mt-2 text-sm text-muted-foreground">{encaminhamento.motivo}</p>

              <div className="mt-3 flex flex-wrap gap-2">
                <Button
                  size="sm"
                  className="bg-sus-green hover:bg-sus-green-dark"
                  onClick={() => aceitarMutation.mutate(encaminhamento.id)}
                  disabled={aceitarMutation.isPending}
                >
                  <Check className="size-3.5" />
                  Aceitar
                </Button>
                <Button
                  size="sm"
                  variant="outline"
                  className="text-sus-red hover:text-sus-red"
                  onClick={() => setEncaminhamentoRecusando(encaminhamento.id)}
                >
                  <X className="size-3.5" />
                  Recusar
                </Button>
                <Button size="sm" variant="outline" onClick={() => navigate(`/pacientes/${encaminhamento.personId}`)}>
                  <Eye className="size-3.5" />
                  Ver ficha
                </Button>
              </div>
            </div>
          ))}
        </div>
      )}

      <Dialog open={!!encaminhamentoRecusando} onOpenChange={(open) => !open && setEncaminhamentoRecusando(null)}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Recusar encaminhamento</DialogTitle>
          </DialogHeader>
          <Field>
            <FieldLabel htmlFor="motivoRecusa">Motivo da recusa (obrigatorio)</FieldLabel>
            <Textarea id="motivoRecusa" rows={3} value={motivoRecusa} onChange={(e) => setMotivoRecusa(e.target.value)} />
          </Field>
          <DialogFooter>
            <Button
              className="bg-sus-red hover:bg-sus-red-dark"
              disabled={!motivoRecusa.trim() || recusarMutation.isPending}
              onClick={() =>
                encaminhamentoRecusando && recusarMutation.mutate({ id: encaminhamentoRecusando, motivo: motivoRecusa })
              }
            >
              {recusarMutation.isPending ? "Enviando..." : "Confirmar recusa"}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
}
