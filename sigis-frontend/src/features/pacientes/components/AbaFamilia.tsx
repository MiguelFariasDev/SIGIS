import { useQuery } from "@tanstack/react-query";
import { Pencil, UsersRound } from "lucide-react";
import { useState } from "react";
import { EmptyState } from "@/components/common/EmptyState";
import { ErrorState } from "@/components/common/ErrorState";
import { LoadingState } from "@/components/common/LoadingState";
import { Button } from "@/components/ui/button";
import { fetchComposicaoFamiliar } from "../api";
import { EditFamilyCompositionModal } from "./EditFamilyCompositionModal";

interface AbaFamiliaProps {
  pessoaId: string;
}

export function AbaFamilia({ pessoaId }: AbaFamiliaProps) {
  const [modalAberto, setModalAberto] = useState(false);

  const composicaoQuery = useQuery({
    queryKey: ["pessoas", pessoaId, "composicao-familiar"],
    queryFn: () => fetchComposicaoFamiliar(pessoaId),
  });

  if (composicaoQuery.isLoading) return <LoadingState />;
  if (composicaoQuery.isError) return <ErrorState onRetry={() => composicaoQuery.refetch()} />;

  const composicao = composicaoQuery.data;

  return (
    <div>
      <div className="mb-3 flex justify-end">
        <Button variant="outline" onClick={() => setModalAberto(true)}>
          <Pencil className="size-4" />
          Editar composicao familiar
        </Button>
      </div>

      {!composicao ? (
        <EmptyState icon={UsersRound} title="Composicao familiar nao preenchida" description="Nenhum dado de composicao familiar registrado." />
      ) : (
        <dl className="grid grid-cols-1 gap-4 rounded-xl border border-border bg-white p-5 sm:grid-cols-2">
          <div>
            <dt className="text-xs text-muted-foreground">Pai</dt>
            <dd className="text-sm text-foreground">
              {composicao.fatherName || "-"}
              {composicao.fatherOccupation && ` — ${composicao.fatherOccupation}`}
            </dd>
          </div>
          <div>
            <dt className="text-xs text-muted-foreground">Mae</dt>
            <dd className="text-sm text-foreground">
              {composicao.motherName || "-"}
              {composicao.motherOccupation && ` — ${composicao.motherOccupation}`}
            </dd>
          </div>
          <div>
            <dt className="text-xs text-muted-foreground">Irmaos</dt>
            <dd className="text-sm text-foreground">
              {composicao.siblingsCount ?? "-"}
              {composicao.siblingsAges && ` (${composicao.siblingsAges})`}
            </dd>
          </div>
          <div>
            <dt className="text-xs text-muted-foreground">Pessoas no domicilio</dt>
            <dd className="text-sm text-foreground">{composicao.householdMembersCount ?? "-"}</dd>
          </div>
          <div>
            <dt className="text-xs text-muted-foreground">Estado civil dos pais</dt>
            <dd className="text-sm text-foreground">{composicao.parentsMaritalStatus || "-"}</dd>
          </div>
          <div>
            <dt className="text-xs text-muted-foreground">Tipo de filiacao</dt>
            <dd className="text-sm text-foreground">{composicao.filiationType || "-"}</dd>
          </div>
          <div>
            <dt className="text-xs text-muted-foreground">Gravidez planejada</dt>
            <dd className="text-sm text-foreground">{composicao.plannedPregnancy ? "Sim" : "Nao"}</dd>
          </div>
          <div>
            <dt className="text-xs text-muted-foreground">Tipo de parto</dt>
            <dd className="text-sm text-foreground">{composicao.deliveryType || "-"}</dd>
          </div>
          {composicao.pregnancyHealthIssue && (
            <div className="sm:col-span-2">
              <dt className="text-xs text-muted-foreground">Intercorrencias na gestacao</dt>
              <dd className="text-sm text-foreground">{composicao.pregnancyHealthIssue}</dd>
            </div>
          )}
        </dl>
      )}

      <EditFamilyCompositionModal open={modalAberto} onOpenChange={setModalAberto} personId={pessoaId} composicao={composicao} />
    </div>
  );
}
