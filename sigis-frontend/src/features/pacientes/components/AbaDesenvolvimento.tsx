import { useQuery } from "@tanstack/react-query";
import { Baby, Pencil } from "lucide-react";
import { useState } from "react";
import { EmptyState } from "@/components/common/EmptyState";
import { ErrorState } from "@/components/common/ErrorState";
import { LoadingState } from "@/components/common/LoadingState";
import { Button } from "@/components/ui/button";
import { fetchDesenvolvimento } from "../api";
import { EditDevelopmentModal } from "./EditDevelopmentModal";

interface AbaDesenvolvimentoProps {
  pessoaId: string;
}

export function AbaDesenvolvimento({ pessoaId }: AbaDesenvolvimentoProps) {
  const [modalAberto, setModalAberto] = useState(false);

  const desenvolvimentoQuery = useQuery({
    queryKey: ["pessoas", pessoaId, "desenvolvimento"],
    queryFn: () => fetchDesenvolvimento(pessoaId),
  });

  if (desenvolvimentoQuery.isLoading) return <LoadingState />;
  if (desenvolvimentoQuery.isError) return <ErrorState onRetry={() => desenvolvimentoQuery.refetch()} />;

  const desenvolvimento = desenvolvimentoQuery.data;

  return (
    <div>
      <div className="mb-3 flex justify-end">
        <Button variant="outline" onClick={() => setModalAberto(true)}>
          <Pencil className="size-4" />
          Editar desenvolvimento
        </Button>
      </div>

      {!desenvolvimento ? (
        <EmptyState icon={Baby} title="Desenvolvimento nao preenchido" description="Nenhum marco de desenvolvimento registrado." />
      ) : (
        <dl className="grid grid-cols-1 gap-4 rounded-xl border border-border bg-white p-5 sm:grid-cols-2">
          <div>
            <dt className="text-xs text-muted-foreground">Idade que andou</dt>
            <dd className="text-sm text-foreground">
              {desenvolvimento.ageWalkedMonths != null ? `${desenvolvimento.ageWalkedMonths} meses` : "-"}
            </dd>
          </div>
          <div>
            <dt className="text-xs text-muted-foreground">Idade que falou</dt>
            <dd className="text-sm text-foreground">
              {desenvolvimento.ageTalkedMonths != null ? `${desenvolvimento.ageTalkedMonths} meses` : "-"}
            </dd>
          </div>
          <div>
            <dt className="text-xs text-muted-foreground">Dominancia manual</dt>
            <dd className="text-sm text-foreground">{desenvolvimento.manualDominance || "-"}</dd>
          </div>
          <div>
            <dt className="text-xs text-muted-foreground">Dificuldades observadas</dt>
            <dd className="text-sm text-foreground">
              {[
                desenvolvimento.locomotionDifficulty && "Locomocao",
                desenvolvimento.coordinationDifficulty && "Coordenacao",
                desenvolvimento.visualDifficulty && "Visual",
                desenvolvimento.hearingDifficulty && "Auditiva",
              ]
                .filter(Boolean)
                .join(", ") || "Nenhuma"}
            </dd>
          </div>
          {desenvolvimento.speechProblems && (
            <div className="sm:col-span-2">
              <dt className="text-xs text-muted-foreground">Problemas de fala</dt>
              <dd className="text-sm text-foreground">{desenvolvimento.speechProblems}</dd>
            </div>
          )}
          {desenvolvimento.communicationForm && (
            <div className="sm:col-span-2">
              <dt className="text-xs text-muted-foreground">Forma de comunicacao</dt>
              <dd className="text-sm text-foreground">{desenvolvimento.communicationForm}</dd>
            </div>
          )}
        </dl>
      )}

      <EditDevelopmentModal
        open={modalAberto}
        onOpenChange={setModalAberto}
        personId={pessoaId}
        desenvolvimento={desenvolvimento}
      />
    </div>
  );
}
