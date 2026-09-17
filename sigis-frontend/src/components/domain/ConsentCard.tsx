import { CheckCircle2, XCircle } from "lucide-react";
import { Button } from "@/components/ui/button";
import type { ConsentType } from "@/lib/types/personConsent";
import type { Secretariat } from "@/lib/types/legalBasis";
import { formatarDataHora } from "@/lib/utils/formatters";
import { SecretariatChip } from "./SecretariatChip";

export interface ConsentCardData {
  id: string;
  type: ConsentType;
  granted: boolean;
  grantedAt: string;
  revokedAt?: string;
  version: string;
  evidence?: string;
}

const TYPE_LABEL: Record<ConsentType, string> = {
  Clinical: "Clinico (Saude)",
  Educational: "Educacional",
  SocialAssistance: "Assistencia Social",
  Research: "Pesquisa",
};

const TYPE_SECRETARIAT: Record<ConsentType, Secretariat | undefined> = {
  Clinical: "Health",
  Educational: "Education",
  SocialAssistance: "SocialAssistance",
  Research: undefined,
};

interface ConsentCardProps {
  consent: ConsentCardData;
  onRevoke?: () => void;
  isRevoking?: boolean;
}

/** T17 — cartao de consentimento LGPD (concedido/revogado), com base legal e evidencia. */
export function ConsentCard({ consent, onRevoke, isRevoking }: ConsentCardProps) {
  const secretariat = TYPE_SECRETARIAT[consent.type];

  return (
    <div className="flex flex-col gap-2 rounded-xl border border-border bg-white p-4 sm:flex-row sm:items-start sm:justify-between">
      <div className="min-w-0">
        <div className="flex flex-wrap items-center gap-2">
          <p className="text-sm font-semibold text-foreground">{TYPE_LABEL[consent.type]}</p>
          {secretariat && <SecretariatChip secretariat={secretariat} />}
          {consent.granted ? (
            <span className="inline-flex items-center gap-1 rounded-full border border-sus-green/30 bg-sus-green-light px-2 py-0.5 text-[11px] font-medium text-sus-green-dark">
              <CheckCircle2 className="size-3" />
              Concedido
            </span>
          ) : (
            <span className="inline-flex items-center gap-1 rounded-full border border-sus-red/30 bg-sus-red-light px-2 py-0.5 text-[11px] font-medium text-sus-red-dark">
              <XCircle className="size-3" />
              Revogado
            </span>
          )}
        </div>
        <p className="mt-1 text-xs text-muted-foreground">
          Versao {consent.version} — concedido em {formatarDataHora(consent.grantedAt)}
          {consent.revokedAt && ` — revogado em ${formatarDataHora(consent.revokedAt)}`}
        </p>
        {consent.evidence && <p className="mt-1 text-xs text-muted-foreground/80">{consent.evidence}</p>}
      </div>

      {consent.granted && onRevoke && (
        <Button size="sm" variant="outline" className="shrink-0 text-sus-red hover:text-sus-red" onClick={onRevoke} disabled={isRevoking}>
          Revogar
        </Button>
      )}
    </div>
  );
}
