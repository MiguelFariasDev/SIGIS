import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Plus, ShieldCheck } from "lucide-react";
import { useState } from "react";
import { toast } from "sonner";
import { EmptyState } from "@/components/common/EmptyState";
import { ErrorState } from "@/components/common/ErrorState";
import { LoadingState } from "@/components/common/LoadingState";
import { ConsentCard } from "@/components/domain/ConsentCard";
import { Button } from "@/components/ui/button";
import { fetchConsentimentos, revogarConsentimento } from "../api";
import { GrantConsentModal } from "./GrantConsentModal";

interface AbaConsentimentosProps {
  pessoaId: string;
}

export function AbaConsentimentos({ pessoaId }: AbaConsentimentosProps) {
  const [modalAberto, setModalAberto] = useState(false);
  const queryClient = useQueryClient();

  const consentimentosQuery = useQuery({
    queryKey: ["pessoas", pessoaId, "consentimentos"],
    queryFn: () => fetchConsentimentos(pessoaId),
  });

  const revogarMutation = useMutation({
    mutationFn: (consentId: string) => revogarConsentimento(pessoaId, consentId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["pessoas", pessoaId, "consentimentos"] });
      toast.success("Consentimento revogado.");
    },
  });

  if (consentimentosQuery.isLoading) return <LoadingState />;
  if (consentimentosQuery.isError) return <ErrorState onRetry={() => consentimentosQuery.refetch()} />;

  return (
    <div>
      <div className="mb-3 flex justify-end">
        <Button variant="outline" onClick={() => setModalAberto(true)}>
          <Plus className="size-4" />
          Registrar consentimento
        </Button>
      </div>

      {consentimentosQuery.data && consentimentosQuery.data.length === 0 && (
        <EmptyState icon={ShieldCheck} title="Nenhum consentimento registrado" description="Esta pessoa ainda nao possui consentimentos LGPD registrados." />
      )}

      {consentimentosQuery.data && consentimentosQuery.data.length > 0 && (
        <div className="space-y-2">
          {consentimentosQuery.data.map((consent) => (
            <ConsentCard
              key={consent.id}
              consent={consent}
              onRevoke={() => revogarMutation.mutate(consent.id)}
              isRevoking={revogarMutation.isPending}
            />
          ))}
        </div>
      )}

      <GrantConsentModal open={modalAberto} onOpenChange={setModalAberto} personId={pessoaId} />
    </div>
  );
}
