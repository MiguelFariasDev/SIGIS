import { useState } from "react";
import { ErrorState } from "@/components/common/ErrorState";
import { LoadingState } from "@/components/common/LoadingState";
import { Timeline } from "@/components/domain/Timeline";
import type { Secretariat } from "@/lib/types/legalBasis";
import { useTimeline } from "../hooks";

interface AbaTimelineProps {
  pessoaId: string;
}

export function AbaTimeline({ pessoaId }: AbaTimelineProps) {
  const [secretariasLiberadas, setSecretariasLiberadas] = useState<Secretariat[]>([]);
  const timelineQuery = useTimeline(pessoaId, secretariasLiberadas);

  if (timelineQuery.isLoading) return <LoadingState />;
  if (timelineQuery.isError || !timelineQuery.data) return <ErrorState onRetry={() => timelineQuery.refetch()} />;

  return (
    <Timeline
      eventos={timelineQuery.data.eventos}
      hiddenCount={timelineQuery.data.hiddenCount}
      personId={pessoaId}
      onAccessGranted={(secretariat) =>
        setSecretariasLiberadas((atual) => (atual.includes(secretariat) ? atual : [...atual, secretariat]))
      }
    />
  );
}
