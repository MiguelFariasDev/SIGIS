import { Badge } from "@/components/ui/badge";
import { cn } from "@/lib/utils";
import { type ServicoSigla, SERVICO_LABEL, SERVICO_SECRETARIA, SECRETARIA_COLOR } from "@/lib/utils/constants";

interface ServiceChipProps {
  sigla: string;
  className?: string;
}

export function ServiceChip({ sigla, className }: ServiceChipProps) {
  const chave = sigla as ServicoSigla;
  const label = SERVICO_LABEL[chave] ?? sigla;
  const secretaria = SERVICO_SECRETARIA[chave];
  const cor = secretaria ? SECRETARIA_COLOR[secretaria] : "bg-neutral-100 text-neutral-600 border-neutral-300";

  return (
    <Badge variant="outline" className={cn("font-medium", cor, className)}>
      {label}
    </Badge>
  );
}
