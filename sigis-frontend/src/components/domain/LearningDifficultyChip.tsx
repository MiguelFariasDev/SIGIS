import { Badge } from "@/components/ui/badge";
import type { LearningDifficultySeverity, LearningDifficultyType } from "@/lib/types/learningDifficulties";
import { cn } from "@/lib/utils";

const TYPE_LABEL: Record<LearningDifficultyType, string> = {
  Leitura: "Leitura",
  Escrita: "Escrita",
  Calculo: "Calculo",
  Atencao: "Atencao",
  Concentracao: "Concentracao",
  CoordenacaoMotora: "Coordenacao motora",
  Outro: "Outro",
};

const SEVERITY_COLOR: Record<LearningDifficultySeverity, string> = {
  Leve: "bg-sus-blue-light text-sus-blue-dark border-sus-blue/30",
  Moderada: "bg-sus-yellow-light text-sus-yellow-dark border-sus-yellow/40",
  Severa: "bg-sus-red-light text-sus-red-dark border-sus-red/30",
};

interface LearningDifficultyChipProps {
  type: LearningDifficultyType;
  severity?: LearningDifficultySeverity;
  className?: string;
}

export function LearningDifficultyChip({ type, severity, className }: LearningDifficultyChipProps) {
  return (
    <Badge
      variant="outline"
      className={cn(
        "font-medium",
        severity ? SEVERITY_COLOR[severity] : "bg-neutral-100 text-neutral-600 border-neutral-300",
        className,
      )}
    >
      {TYPE_LABEL[type]}
      {severity && ` — ${severity}`}
    </Badge>
  );
}
