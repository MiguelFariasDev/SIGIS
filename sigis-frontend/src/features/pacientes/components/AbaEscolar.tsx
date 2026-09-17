import { useQuery } from "@tanstack/react-query";
import { Plus, School } from "lucide-react";
import { useState } from "react";
import { EmptyState } from "@/components/common/EmptyState";
import { ErrorState } from "@/components/common/ErrorState";
import { LoadingState } from "@/components/common/LoadingState";
import { LearningDifficultyChip } from "@/components/domain/LearningDifficultyChip";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import type { Person } from "@/lib/types/person";
import { formatarData } from "@/lib/utils/formatters";
import { fetchDificuldadesAprendizagem, fetchHistoricoEscolar } from "../api";
import { AddLearningDifficultyModal } from "./AddLearningDifficultyModal";
import { AddSchoolHistoryModal } from "./AddSchoolHistoryModal";

interface AbaEscolarProps {
  pessoa: Person;
}

export function AbaEscolar({ pessoa }: AbaEscolarProps) {
  const [modalHistoricoAberto, setModalHistoricoAberto] = useState(false);
  const [modalDificuldadeAberto, setModalDificuldadeAberto] = useState(false);

  const historicoQuery = useQuery({
    queryKey: ["pessoas", pessoa.id, "historico-escolar"],
    queryFn: () => fetchHistoricoEscolar(pessoa.id),
  });

  const dificuldadesQuery = useQuery({
    queryKey: ["pessoas", pessoa.id, "dificuldades-aprendizagem"],
    queryFn: () => fetchDificuldadesAprendizagem(pessoa.id),
  });

  const disabilityTypes = pessoa.disabilityTypes
    ? pessoa.disabilityTypes.split(",").map((v) => v.trim()).filter(Boolean)
    : [];

  return (
    <div className="space-y-6">
      <div>
        <p className="mb-2 text-sm font-semibold text-foreground">Dados escolares atuais</p>
        {!pessoa.currentSchool && !pessoa.grade ? (
          <EmptyState icon={School} title="Sem vinculo escolar registrado" description="Esta pessoa nao possui dados escolares no cadastro." />
        ) : (
          <dl className="grid grid-cols-1 gap-4 rounded-xl border border-border bg-white p-5 sm:grid-cols-2">
            <div>
              <dt className="text-xs text-muted-foreground">Escola atual</dt>
              <dd className="text-sm text-foreground">{pessoa.currentSchool || "-"}</dd>
            </div>
            <div>
              <dt className="text-xs text-muted-foreground">Serie/turno/turma</dt>
              <dd className="text-sm text-foreground">
                {[pessoa.grade, pessoa.shift, pessoa.classGroup].filter(Boolean).join(" — ") || "-"}
              </dd>
            </div>
            <div>
              <dt className="text-xs text-muted-foreground">Zona</dt>
              <dd className="text-sm text-foreground">{pessoa.zone || "-"}</dd>
            </div>
            <div>
              <dt className="text-xs text-muted-foreground">Matricula escolar</dt>
              <dd className="text-sm text-foreground">{pessoa.schoolEnrollment || "-"}</dd>
            </div>
            <div className="sm:col-span-2 flex flex-wrap gap-1.5">
              {pessoa.referredBySchool && <Badge variant="outline">Encaminhado pela escola</Badge>}
              {pessoa.needsSpecialEducation && <Badge variant="outline">Necessita educacao especial</Badge>}
              {pessoa.attendsTutoring && <Badge variant="outline">Frequenta reforco escolar</Badge>}
              {pessoa.hasFailedGrade && <Badge variant="outline">Ja reprovou de ano</Badge>}
              {disabilityTypes.map((tipo) => (
                <Badge key={tipo} variant="outline">
                  {tipo}
                </Badge>
              ))}
            </div>
          </dl>
        )}
      </div>

      <div>
        <div className="mb-2 flex items-center justify-between">
          <p className="text-sm font-semibold text-foreground">Historico escolar</p>
          <Button size="sm" variant="outline" onClick={() => setModalHistoricoAberto(true)}>
            <Plus className="size-3.5" />
            Adicionar
          </Button>
        </div>

        {historicoQuery.isLoading && <LoadingState />}
        {historicoQuery.isError && <ErrorState onRetry={() => historicoQuery.refetch()} />}
        {historicoQuery.data && historicoQuery.data.length === 0 && (
          <EmptyState icon={School} title="Nenhum historico escolar" description="Nenhum registro de historico escolar para esta pessoa." />
        )}
        {historicoQuery.data && historicoQuery.data.length > 0 && (
          <div className="space-y-2">
            {historicoQuery.data.map((item) => (
              <div key={item.id} className="rounded-xl border border-border bg-white p-4">
                <div className="flex flex-wrap items-center justify-between gap-2">
                  <p className="text-sm font-medium text-foreground">
                    {item.schoolName} — {item.grade} ({item.schoolYear})
                  </p>
                  <Badge variant="outline">{item.status}</Badge>
                </div>
                <p className="mt-1 text-xs text-muted-foreground">
                  {[item.shift, item.classGroup].filter(Boolean).join(" — ")}
                  {item.startDate && ` — desde ${formatarData(item.startDate)}`}
                </p>
                {item.notes && <p className="mt-1 text-xs text-muted-foreground/80">{item.notes}</p>}
              </div>
            ))}
          </div>
        )}
      </div>

      <div>
        <div className="mb-2 flex items-center justify-between">
          <p className="text-sm font-semibold text-foreground">Dificuldades de aprendizagem</p>
          <Button size="sm" variant="outline" onClick={() => setModalDificuldadeAberto(true)}>
            <Plus className="size-3.5" />
            Adicionar
          </Button>
        </div>

        {dificuldadesQuery.isLoading && <LoadingState />}
        {dificuldadesQuery.isError && <ErrorState onRetry={() => dificuldadesQuery.refetch()} />}
        {dificuldadesQuery.data && dificuldadesQuery.data.length === 0 && (
          <p className="text-sm text-muted-foreground">Nenhuma dificuldade de aprendizagem registrada.</p>
        )}
        {dificuldadesQuery.data && dificuldadesQuery.data.length > 0 && (
          <div className="flex flex-wrap gap-2">
            {dificuldadesQuery.data.map((d) => (
              <LearningDifficultyChip key={d.id} type={d.type} severity={d.severity} />
            ))}
          </div>
        )}
      </div>

      <AddSchoolHistoryModal open={modalHistoricoAberto} onOpenChange={setModalHistoricoAberto} personId={pessoa.id} />
      <AddLearningDifficultyModal open={modalDificuldadeAberto} onOpenChange={setModalDificuldadeAberto} personId={pessoa.id} />
    </div>
  );
}
