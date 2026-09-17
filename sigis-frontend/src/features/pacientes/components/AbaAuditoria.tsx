import { useQuery } from "@tanstack/react-query";
import { ErrorState } from "@/components/common/ErrorState";
import { LoadingState } from "@/components/common/LoadingState";
import { fetchAuditoria } from "@/features/auditoria/api";
import { LogTable } from "@/features/auditoria/components/LogTable";

interface AbaAuditoriaProps {
  pessoaId: string;
}

export function AbaAuditoria({ pessoaId }: AbaAuditoriaProps) {
  const auditoriaQuery = useQuery({
    queryKey: ["auditoria", "pessoa", pessoaId],
    queryFn: () => fetchAuditoria({ personId: pessoaId }),
  });

  if (auditoriaQuery.isLoading) return <LoadingState />;
  if (auditoriaQuery.isError || !auditoriaQuery.data) return <ErrorState onRetry={() => auditoriaQuery.refetch()} />;

  return <LogTable logs={auditoriaQuery.data} />;
}
