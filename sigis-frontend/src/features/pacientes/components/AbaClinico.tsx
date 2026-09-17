import { useQuery } from "@tanstack/react-query";
import { Pencil, Stethoscope } from "lucide-react";
import { useState } from "react";
import { EmptyState } from "@/components/common/EmptyState";
import { ErrorState } from "@/components/common/ErrorState";
import { LoadingState } from "@/components/common/LoadingState";
import { Button } from "@/components/ui/button";
import { unidadesMock } from "@/lib/mocks/data/unidades.mock";
import { fetchPerfilClinico } from "../api";
import { EditClinicalProfileModal } from "./EditClinicalProfileModal";

interface AbaClinicoProps {
  pessoaId: string;
}

export function AbaClinico({ pessoaId }: AbaClinicoProps) {
  const [modalAberto, setModalAberto] = useState(false);

  const perfilQuery = useQuery({
    queryKey: ["pessoas", pessoaId, "perfil-clinico"],
    queryFn: () => fetchPerfilClinico(pessoaId),
  });

  if (perfilQuery.isLoading) return <LoadingState />;
  if (perfilQuery.isError) return <ErrorState onRetry={() => perfilQuery.refetch()} />;

  const perfil = perfilQuery.data;
  const unidade = unidadesMock.find((u) => u.id === perfil?.apsReferenceUnitId);

  return (
    <div>
      <div className="mb-3 flex justify-end">
        <Button variant="outline" onClick={() => setModalAberto(true)}>
          <Pencil className="size-4" />
          Editar perfil clinico
        </Button>
      </div>

      {!perfil ? (
        <EmptyState icon={Stethoscope} title="Perfil clinico nao preenchido" description="Nenhum dado clinico NASF registrado para esta pessoa." />
      ) : (
        <dl className="grid grid-cols-1 gap-4 rounded-xl border border-border bg-white p-5 sm:grid-cols-2">
          <div>
            <dt className="text-xs text-muted-foreground">Numero do prontuario</dt>
            <dd className="text-sm text-foreground">{perfil.medicalRecordNumber || "-"}</dd>
          </div>
          <div>
            <dt className="text-xs text-muted-foreground">Unidade de referencia (APS)</dt>
            <dd className="text-sm text-foreground">{unidade ? `${unidade.nome} (${unidade.sigla})` : "-"}</dd>
          </div>
          <div className="sm:col-span-2">
            <dt className="text-xs text-muted-foreground">Hipotese clinica</dt>
            <dd className="text-sm text-foreground">{perfil.clinicalHypothesis || "-"}</dd>
          </div>
        </dl>
      )}

      <EditClinicalProfileModal open={modalAberto} onOpenChange={setModalAberto} personId={pessoaId} perfil={perfil} />
    </div>
  );
}
