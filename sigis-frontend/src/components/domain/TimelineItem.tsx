import { ListOrdered, Lock, Send, ShieldCheck, Stethoscope, UserPlus, UserX } from "lucide-react";
import { Button } from "@/components/ui/button";
import type { TimelineEvento, TimelineEventoTipo } from "@/lib/types/timeline";
import { formatarDataHora } from "@/lib/utils/formatters";
import { ServiceChip } from "./ServiceChip";
import { SecretariatChip } from "./SecretariatChip";

const ICONE_POR_TIPO: Record<TimelineEventoTipo, typeof Stethoscope> = {
  CADASTRO: UserPlus,
  ATENDIMENTO: Stethoscope,
  ENCAMINHAMENTO_ENVIADO: Send,
  ENCAMINHAMENTO_RECEBIDO: Send,
  ENTRADA_FILA: ListOrdered,
  FALTA: UserX,
  ACESSO_CROSS_UNIDADE: ShieldCheck,
};

interface TimelineItemProps {
  evento: TimelineEvento;
  isLast?: boolean;
  /** Aciona a solicitacao de acesso (T15) para a secretaria do evento bloqueado. */
  onRequestAccess?: () => void;
}

/**
 * T06 aba Timeline — icone por tipo, cor por secretaria (RN), cadeado
 * quando o evento e cross-secretaria e ainda nao houve solicitacao de
 * acesso aprovada (RF12), e badge quando o acesso ja foi registrado.
 */
export function TimelineItem({ evento, isLast = false, onRequestAccess }: TimelineItemProps) {
  const Icone = evento.locked ? Lock : ICONE_POR_TIPO[evento.tipo];

  return (
    <div className="flex gap-4">
      <div className="flex flex-col items-center">
        <div
          className={`flex size-9 shrink-0 items-center justify-center rounded-full border ${
            evento.locked
              ? "border-neutral-300 bg-neutral-100 text-neutral-500"
              : "border-transparent bg-sus-blue-light text-sus-blue"
          }`}
        >
          <Icone className="size-4" />
        </div>
        {!isLast && <div className="mt-1 w-px flex-1 bg-border" />}
      </div>
      <div className="min-w-0 flex-1 pb-6">
        <div className="flex flex-wrap items-center gap-2">
          <p className={`text-sm font-semibold ${evento.locked ? "text-muted-foreground" : "text-foreground"}`}>
            {evento.titulo}
          </p>
          <SecretariatChip secretariat={evento.secretariat} />
          {evento.unidadeSigla && !evento.locked && <ServiceChip sigla={evento.unidadeSigla} />}
          {evento.acessoRegistrado && (
            <span className="inline-flex items-center gap-1 rounded-full border border-sus-blue/30 bg-sus-blue-light px-2 py-0.5 text-[11px] font-medium text-sus-blue-dark">
              <ShieldCheck className="size-3" />
              Acesso registrado
            </span>
          )}
        </div>
        {evento.descricao && <p className="mt-0.5 text-sm text-muted-foreground">{evento.descricao}</p>}
        {evento.locked && (
          <div className="mt-1.5 flex flex-wrap items-center gap-2">
            <p className="text-xs text-muted-foreground">
              Conteudo oculto — pertence a outra secretaria (RF12).
            </p>
            {onRequestAccess && (
              <Button size="sm" variant="outline" className="h-6 px-2 text-[11px]" onClick={onRequestAccess}>
                <Lock className="size-3" />
                Solicitar acesso
              </Button>
            )}
          </div>
        )}
        <p className="mt-1 text-xs text-muted-foreground/80">{formatarDataHora(evento.dataHora)}</p>
      </div>
    </div>
  );
}
