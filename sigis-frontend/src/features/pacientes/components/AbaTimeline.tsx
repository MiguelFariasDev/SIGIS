import { Lock } from "lucide-react";
import { useState } from "react";
import { EmptyState } from "@/components/common/EmptyState";
import { ErrorState } from "@/components/common/ErrorState";
import { LoadingState } from "@/components/common/LoadingState";
import { RequestAccessModal } from "@/components/domain/RequestAccessModal";
import { Timeline } from "@/components/domain/Timeline";
import { Button } from "@/components/ui/button";
import { ForbiddenError } from "@/lib/api/errors";
import { useTimeline } from "../hooks";

interface AbaTimelineProps {
  pessoaId: string;
}

/**
 * O backend real desbloqueia a linha do tempo inteira por chamada (nível
 * "metadados"/"completo" + justificativa no próprio GET), não por
 * secretaria — uma vez concedido, refazemos a consulta em nível "completo".
 * Mesmo o nível "metadados" retorna 403 quando a pessoa tem histórico em
 * outra secretaria e nenhuma justificativa foi informada (RF12) — por isso
 * tratamos esse erro como "bloqueado, peça acesso", não como falha genérica.
 */
export function AbaTimeline({ pessoaId }: AbaTimelineProps) {
  const [acesso, setAcesso] = useState<{ completo: boolean; justificativa?: string }>({ completo: false });
  const [modalAberto, setModalAberto] = useState(false);
  const timelineQuery = useTimeline(pessoaId, acesso);

  if (timelineQuery.isLoading) return <LoadingState />;

  if (timelineQuery.isError) {
    if (timelineQuery.error instanceof ForbiddenError) {
      return (
        <>
          <EmptyState
            icon={Lock}
            title="Linha do tempo bloqueada"
            description="Esta pessoa possui historico em outra secretaria. Informe a justificativa de acesso para visualizar (LGPD, art. 11, II)."
            action={<Button onClick={() => setModalAberto(true)}>Solicitar acesso</Button>}
          />
          <RequestAccessModal
            open={modalAberto}
            onOpenChange={setModalAberto}
            personId={pessoaId}
            onGranted={(_secretariat, justificativa) => setAcesso({ completo: true, justificativa })}
          />
        </>
      );
    }
    return <ErrorState onRetry={() => timelineQuery.refetch()} />;
  }

  if (!timelineQuery.data) return <ErrorState onRetry={() => timelineQuery.refetch()} />;

  return (
    <Timeline
      eventos={timelineQuery.data.eventos}
      hiddenCount={timelineQuery.data.hiddenCount}
      personId={pessoaId}
      onAccessGranted={(justificativa) => setAcesso({ completo: true, justificativa })}
    />
  );
}
