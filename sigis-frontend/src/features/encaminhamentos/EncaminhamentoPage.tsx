import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useNavigate, useSearchParams } from "react-router-dom";
import { toast } from "sonner";
import { PageHeader } from "@/components/common/PageHeader";
import type { CriarEncaminhamentoRequest } from "@/lib/types/encaminhamento";
import { criarEncaminhamento } from "./api";
import { EncaminhamentoForm } from "./components/EncaminhamentoForm";

export function EncaminhamentoPage() {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const queryClient = useQueryClient();

  const pessoaIdInicial = searchParams.get("pessoaId") ?? undefined;
  const pessoaNomeInicial = searchParams.get("pessoaNome") ?? undefined;

  const criarMutation = useMutation({
    mutationFn: criarEncaminhamento,
    onSuccess: (encaminhamento) => {
      queryClient.invalidateQueries({ queryKey: ["indicadores"] });
      toast.success("Encaminhamento registrado com sucesso.");
      navigate(`/encaminhamentos/${encaminhamento.id}/rastreio`);
    },
  });

  function handleSubmit(request: CriarEncaminhamentoRequest) {
    criarMutation.mutate(request);
  }

  return (
    <div className="mx-auto max-w-xl">
      <PageHeader
        title="Novo encaminhamento"
        description="Encaminhe o paciente para outra unidade da rede, com motivo e prioridade."
      />
      <div className="rounded-xl border border-border bg-white p-6">
        <EncaminhamentoForm
          pessoaIdInicial={pessoaIdInicial}
          pessoaNomeInicial={pessoaNomeInicial}
          onSubmit={handleSubmit}
          isSubmitting={criarMutation.isPending}
        />
      </div>
    </div>
  );
}
