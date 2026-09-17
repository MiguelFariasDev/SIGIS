import { Badge } from "@/components/ui/badge";
import type { Secretariat } from "@/lib/types/legalBasis";
import { cn } from "@/lib/utils";
import { SECRETARIAT_COLOR, SECRETARIAT_EMOJI, SECRETARIAT_LABEL } from "@/lib/utils/constants";

interface SecretariatChipProps {
  secretariat: Secretariat;
  className?: string;
}

/** Identifica visualmente a secretaria responsavel por um evento/registro (RN — paleta oficial). */
export function SecretariatChip({ secretariat, className }: SecretariatChipProps) {
  return (
    <Badge variant="outline" className={cn("gap-1 font-medium", SECRETARIAT_COLOR[secretariat], className)}>
      <span aria-hidden="true">{SECRETARIAT_EMOJI[secretariat]}</span>
      {SECRETARIAT_LABEL[secretariat]}
    </Badge>
  );
}
