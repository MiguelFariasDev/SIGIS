import { Badge } from "@/components/ui/badge";
import type { StatusAlertaDuplicidade, StatusComparecimento, StatusEncaminhamento, StatusFila } from "@/lib/types/enums";
import { cn } from "@/lib/utils";
import {
  COMPARECIMENTO_LABEL,
  STATUS_DUPLICIDADE_LABEL,
  STATUS_ENCAMINHAMENTO_COLOR,
  STATUS_ENCAMINHAMENTO_LABEL,
  STATUS_FILA_COLOR,
  STATUS_FILA_LABEL,
} from "@/lib/utils/constants";

type StatusBadgeProps =
  | { kind: "fila"; status: StatusFila; className?: string }
  | { kind: "encaminhamento"; status: StatusEncaminhamento; className?: string }
  | { kind: "duplicidade"; status: StatusAlertaDuplicidade; className?: string }
  | { kind: "comparecimento"; status: StatusComparecimento; className?: string };

const COMPARECIMENTO_COLOR: Record<StatusComparecimento, string> = {
  AGENDADO: "bg-sus-blue-light text-sus-blue-dark border-sus-blue/30",
  COMPARECEU: "bg-sus-green-light text-sus-green-dark border-sus-green/30",
  FALTOU: "bg-sus-red-light text-sus-red-dark border-sus-red/30",
};

const DUPLICIDADE_COLOR: Record<StatusAlertaDuplicidade, string> = {
  PENDENTE: "bg-sus-yellow-light text-sus-yellow-dark border-sus-yellow/40",
  MESCLADO: "bg-sus-green-light text-sus-green-dark border-sus-green/30",
  FALSO_POSITIVO: "bg-neutral-100 text-neutral-600 border-neutral-300",
};

function resolverLabelECor(props: StatusBadgeProps): [string, string] {
  switch (props.kind) {
    case "fila":
      return [STATUS_FILA_LABEL[props.status], STATUS_FILA_COLOR[props.status]];
    case "encaminhamento":
      return [STATUS_ENCAMINHAMENTO_LABEL[props.status], STATUS_ENCAMINHAMENTO_COLOR[props.status]];
    case "duplicidade":
      return [STATUS_DUPLICIDADE_LABEL[props.status], DUPLICIDADE_COLOR[props.status]];
    case "comparecimento":
      return [COMPARECIMENTO_LABEL[props.status], COMPARECIMENTO_COLOR[props.status]];
  }
}

export function StatusBadge(props: StatusBadgeProps) {
  const [label, color] = resolverLabelECor(props);

  return (
    <Badge variant="outline" className={cn("font-medium", color, props.className)}>
      {label}
    </Badge>
  );
}
