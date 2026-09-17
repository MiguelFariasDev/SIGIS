import { DndContext, type DragEndEvent, PointerSensor, useSensor, useSensors } from "@dnd-kit/core";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { LayoutGrid, ListFilter } from "lucide-react";
import { useMemo, useState } from "react";
import { Navigate, useParams } from "react-router-dom";
import { toast } from "sonner";
import { ErrorState } from "@/components/common/ErrorState";
import { LoadingState } from "@/components/common/LoadingState";
import { PageHeader } from "@/components/common/PageHeader";
import { Input } from "@/components/ui/input";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { unidadesMock } from "@/lib/mocks/data/unidades.mock";
import { PapelRbac, StatusFila } from "@/lib/types/enums";
import { cn } from "@/lib/utils";
import { useAuthStore } from "@/stores/authStore";
import { atualizarStatusFila, chamarProximo, fetchFilaPorUnidade, fetchFilaRede } from "./api";
import { ChamarProximoButton } from "./components/ChamarProximoButton";
import { KanbanColuna } from "./components/KanbanColuna";

const COLUNAS_STATUS = [StatusFila.AGUARDANDO, StatusFila.EM_ATENDIMENTO, StatusFila.CONCLUIDO, StatusFila.FALTOU];
const STATUS_LABEL: Record<StatusFila, string> = {
  AGUARDANDO: "Aguardando",
  EM_ATENDIMENTO: "Em atendimento",
  CONCLUIDO: "Concluido",
  FALTOU: "Faltou",
  BUSCA_ATIVA: "Busca ativa",
};
const REDE_INTEIRA = "REDE_INTEIRA";
const TODAS_ESPECIALIDADES = "TODAS";

type ModoVisualizacao = "prioridade" | "especialidade";

/**
 * T07 — Fila com filtros hierarquicos (RF-Fila): Nivel 1 Rede/Unidade,
 * Nivel 2 Especialidade, Nivel 3 busca por nome. PROFISSIONAL fica preso
 * a propria unidade; COORDENADOR enxerga a rede inteira; AUDITOR nao tem
 * acesso a fila (papel e apenas de auditoria/DPO).
 */
export function FilaPage() {
  const { unidadeId: unidadeIdRota } = useParams<{ unidadeId: string }>();
  const usuario = useAuthStore((s) => s.usuario);
  const queryClient = useQueryClient();
  const sensors = useSensors(useSensor(PointerSensor, { activationConstraint: { distance: 6 } }));

  const podeVerRede = usuario?.papelRbac === PapelRbac.COORDENADOR;
  const unidadeTravada = usuario?.papelRbac === PapelRbac.PROFISSIONAL;

  const [unidadeSelecionada, setUnidadeSelecionada] = useState<string>(
    unidadeIdRota ?? usuario?.unidadeId ?? REDE_INTEIRA,
  );
  const [especialidadeSelecionada, setEspecialidadeSelecionada] = useState(TODAS_ESPECIALIDADES);
  const [busca, setBusca] = useState("");
  const [modoVisualizacao, setModoVisualizacao] = useState<ModoVisualizacao>("prioridade");

  const modoRede = podeVerRede && unidadeSelecionada === REDE_INTEIRA;
  const unidade = unidadesMock.find((u) => u.id === unidadeSelecionada);

  const filaQuery = useQuery({
    queryKey: ["filas", modoRede ? "rede" : "unidade", unidadeSelecionada],
    queryFn: () =>
      modoRede ? fetchFilaRede() : fetchFilaPorUnidade(unidadeSelecionada),
    enabled: !!unidadeSelecionada,
  });

  const atualizarStatusMutation = useMutation({
    mutationFn: ({ id, status }: { id: string; status: StatusFila }) => atualizarStatusFila(id, status),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["filas"] });
    },
  });

  const chamarProximoMutation = useMutation({
    mutationFn: () => chamarProximo(unidadeSelecionada),
    onSuccess: (item) => {
      queryClient.invalidateQueries({ queryKey: ["filas"] });
      toast.success(`${item.personName} chamado(a) para atendimento.`);
    },
    onError: () => {
      toast.info("Nao ha pacientes aguardando nesta fila.");
    },
  });

  const especialidadesDisponiveis = useMemo(
    () => Array.from(new Set((filaQuery.data ?? []).map((item) => item.especialidade))).sort(),
    [filaQuery.data],
  );

  const itensFiltrados = useMemo(() => {
    const termo = busca.trim().toLowerCase();
    return (filaQuery.data ?? []).filter((item) => {
      const passaEspecialidade =
        especialidadeSelecionada === TODAS_ESPECIALIDADES || item.especialidade === especialidadeSelecionada;
      const passaBusca = termo === "" || item.personName.toLowerCase().includes(termo);
      return passaEspecialidade && passaBusca;
    });
  }, [filaQuery.data, especialidadeSelecionada, busca]);

  if (usuario?.papelRbac === PapelRbac.AUDITOR) {
    return <Navigate to="/dashboard" replace />;
  }

  function handleDragEnd(event: DragEndEvent) {
    if (modoVisualizacao !== "prioridade") return;
    const { active, over } = event;
    if (!over) return;
    const novoStatus = over.id as StatusFila;
    const item = itensFiltrados.find((f) => f.id === active.id);
    if (item && item.status !== novoStatus) {
      atualizarStatusMutation.mutate({ id: item.id, status: novoStatus });
    }
  }

  return (
    <div>
      <PageHeader
        title="Fila"
        description={
          modoRede
            ? `${itensFiltrados.length} pessoa(s) na fila da rede.`
            : `${itensFiltrados.length} pessoa(s) na fila de ${unidade?.nome ?? "unidade"}.`
        }
        actions={
          !modoRede && (
            <ChamarProximoButton
              onClick={() => chamarProximoMutation.mutate()}
              disabled={chamarProximoMutation.isPending}
            />
          )
        }
      />

      <div className="mb-4 flex flex-wrap items-end gap-3">
        {unidadeTravada ? (
          <div className="text-sm text-muted-foreground">
            Unidade: <span className="font-medium text-foreground">{unidade?.nome ?? unidadeSelecionada}</span>
          </div>
        ) : (
          <div className="w-56">
            <label className="mb-1 block text-xs font-medium text-muted-foreground">Rede / Unidade</label>
            <Select value={unidadeSelecionada} onValueChange={setUnidadeSelecionada}>
              <SelectTrigger className="w-full">
                <SelectValue />
              </SelectTrigger>
              <SelectContent>
                {podeVerRede && <SelectItem value={REDE_INTEIRA}>Rede inteira</SelectItem>}
                {unidadesMock.map((u) => (
                  <SelectItem key={u.id} value={u.id}>
                    {u.nome} ({u.sigla})
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>
        )}

        <div className="w-56">
          <label className="mb-1 block text-xs font-medium text-muted-foreground">Especialidade</label>
          <Select value={especialidadeSelecionada} onValueChange={setEspecialidadeSelecionada}>
            <SelectTrigger className="w-full">
              <SelectValue />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value={TODAS_ESPECIALIDADES}>Todas as especialidades</SelectItem>
              {especialidadesDisponiveis.map((esp) => (
                <SelectItem key={esp} value={esp}>
                  {esp}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
        </div>

        <div className="w-64">
          <label className="mb-1 block text-xs font-medium text-muted-foreground">Buscar por nome</label>
          <Input placeholder="Nome do paciente..." value={busca} onChange={(e) => setBusca(e.target.value)} />
        </div>

        <div className="ml-auto inline-flex rounded-lg border border-border bg-white p-1">
          <button
            type="button"
            onClick={() => setModoVisualizacao("prioridade")}
            className={cn(
              "flex items-center gap-1.5 rounded-md px-3 py-1.5 text-sm font-medium text-muted-foreground transition-colors",
              modoVisualizacao === "prioridade" && "bg-sus-blue text-white",
            )}
          >
            <ListFilter className="size-3.5" />
            Por prioridade
          </button>
          <button
            type="button"
            onClick={() => setModoVisualizacao("especialidade")}
            className={cn(
              "flex items-center gap-1.5 rounded-md px-3 py-1.5 text-sm font-medium text-muted-foreground transition-colors",
              modoVisualizacao === "especialidade" && "bg-sus-blue text-white",
            )}
          >
            <LayoutGrid className="size-3.5" />
            Por especialidade
          </button>
        </div>
      </div>

      {filaQuery.isLoading && <LoadingState />}
      {filaQuery.isError && <ErrorState onRetry={() => filaQuery.refetch()} />}

      {filaQuery.data && (
        <DndContext sensors={sensors} onDragEnd={handleDragEnd}>
          {modoVisualizacao === "prioridade" ? (
            <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-4">
              {COLUNAS_STATUS.map((status) => (
                <KanbanColuna
                  key={status}
                  id={status}
                  titulo={STATUS_LABEL[status]}
                  itens={itensFiltrados.filter((item) => item.status === status)}
                  onAtualizarStatus={(id, novoStatus) => atualizarStatusMutation.mutate({ id, status: novoStatus })}
                  mostrarUnidade={modoRede}
                />
              ))}
            </div>
          ) : (
            <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
              {especialidadesDisponiveis
                .filter((esp) => especialidadeSelecionada === TODAS_ESPECIALIDADES || esp === especialidadeSelecionada)
                .map((esp) => (
                  <KanbanColuna
                    key={esp}
                    id={esp}
                    titulo={esp}
                    droppable={false}
                    itens={itensFiltrados.filter((item) => item.especialidade === esp)}
                    onAtualizarStatus={(id, novoStatus) => atualizarStatusMutation.mutate({ id, status: novoStatus })}
                    mostrarUnidade={modoRede}
                  />
                ))}
            </div>
          )}
        </DndContext>
      )}

      {filaQuery.data && itensFiltrados.length === 0 && (
        <p className="mt-6 text-center text-sm text-muted-foreground">Nenhum item encontrado para os filtros atuais.</p>
      )}
    </div>
  );
}
