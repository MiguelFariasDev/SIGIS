import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { CheckCircle2, Users2 } from "lucide-react";
import { useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { toast } from "sonner";
import { ErrorState } from "@/components/common/ErrorState";
import { LoadingState } from "@/components/common/LoadingState";
import { PageHeader } from "@/components/common/PageHeader";
import { Button } from "@/components/ui/button";
import { formatarPercentual } from "@/lib/utils/formatters";
import { fetchDuplicidade, resolverDuplicidade } from "./api";
import { ComparacaoLadoALado } from "./components/ComparacaoLadoALado";
import { MergeConfirmDialog } from "./components/MergeConfirmDialog";

export function DuplicidadeAlertPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const [mergeDialogAberto, setMergeDialogAberto] = useState(false);

  const duplicidadeQuery = useQuery({
    queryKey: ["duplicidades", id],
    queryFn: () => fetchDuplicidade(id!),
    enabled: !!id,
  });

  const resolverMutation = useMutation({
    mutationFn: (acao: "MESCLAR" | "FALSO_POSITIVO") => resolverDuplicidade(id!, { acao }),
    onSuccess: (_, acao) => {
      queryClient.invalidateQueries({ queryKey: ["duplicidades"] });
      queryClient.invalidateQueries({ queryKey: ["indicadores"] });
      toast.success(acao === "MESCLAR" ? "Cadastros mesclados com sucesso." : "Marcado como falso positivo.");
      navigate("/duplicidades");
    },
  });

  if (duplicidadeQuery.isLoading) return <LoadingState />;
  if (duplicidadeQuery.isError || !duplicidadeQuery.data) {
    return <ErrorState onRetry={() => duplicidadeQuery.refetch()} />;
  }

  const alerta = duplicidadeQuery.data;
  if (!alerta.person1 || !alerta.person2) {
    return <ErrorState title="Dados incompletos para comparacao." />;
  }

  return (
    <div>
      <PageHeader
        title="Revisao de duplicidade"
        description="Compare os dois cadastros e decida se pertencem a mesma pessoa."
      />

      <div className="mb-4 rounded-xl border border-sus-yellow/40 bg-sus-yellow-light p-4">
        <p className="text-sm font-semibold text-sus-yellow-dark">
          {formatarPercentual(alerta.scoreSimilaridade)} similar
        </p>
        <p className="mt-1 text-sm text-sus-yellow-dark/90">{alerta.motivoMatch}</p>
      </div>

      <ComparacaoLadoALado
        tituloA={`Cadastro A — ${alerta.person1.services?.[0] ?? "origem desconhecida"}`}
        tituloB={`Cadastro B — ${alerta.person2.services?.[0] ?? "origem desconhecida"}`}
        pessoaA={alerta.person1}
        pessoaB={alerta.person2}
      />

      <div className="mt-6 flex flex-col gap-3 sm:flex-row">
        <Button
          className="bg-sus-blue hover:bg-sus-blue-dark"
          onClick={() => setMergeDialogAberto(true)}
          disabled={resolverMutation.isPending}
        >
          <CheckCircle2 className="size-4" />
          Confirmar mesma pessoa
        </Button>
        <Button
          variant="outline"
          onClick={() => resolverMutation.mutate("FALSO_POSITIVO")}
          disabled={resolverMutation.isPending}
        >
          <Users2 className="size-4" />
          Sao pessoas diferentes
        </Button>
      </div>

      <MergeConfirmDialog
        open={mergeDialogAberto}
        onOpenChange={setMergeDialogAberto}
        onConfirm={() => resolverMutation.mutate("MESCLAR")}
        nomeA={alerta.person1.fullName}
        nomeB={alerta.person2.fullName}
      />
    </div>
  );
}
