import { useMutation, useQueryClient } from "@tanstack/react-query";
import { Pencil, Send, Stethoscope, User } from "lucide-react";
import { useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { toast } from "sonner";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import { ErrorState } from "@/components/common/ErrorState";
import { LoadingState } from "@/components/common/LoadingState";
import { ServiceChip } from "@/components/domain/ServiceChip";
import { Button } from "@/components/ui/button";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { criarAtendimento } from "@/features/atendimentos/api";
import { ReferralModal } from "@/features/encaminhamentos/components/ReferralModal";
import { PapelRbac, TipoSessao } from "@/lib/types/enums";
import type { ServicoSigla } from "@/lib/utils/constants";
import { formatarData, formatarIdade } from "@/lib/utils/formatters";
import { useAuthStore } from "@/stores/authStore";
import { AbaAtendimentos } from "./components/AbaAtendimentos";
import { AbaAuditoria } from "./components/AbaAuditoria";
import { AbaClinico } from "./components/AbaClinico";
import { AbaConsentimentos } from "./components/AbaConsentimentos";
import { AbaDesenvolvimento } from "./components/AbaDesenvolvimento";
import { AbaEncaminhamentos } from "./components/AbaEncaminhamentos";
import { AbaEscolar } from "./components/AbaEscolar";
import { AbaFamilia } from "./components/AbaFamilia";
import { AbaFila } from "./components/AbaFila";
import { AbaTimeline } from "./components/AbaTimeline";
import { AbaTratamentosConcomitantes } from "./components/AbaTratamentosConcomitantes";
import { usePessoa } from "./hooks";

const TIPO_SESSAO_POR_SERVICO: Record<ServicoSigla, TipoSessao> = {
  NASF: TipoSessao.PRONTUARIO_NASF,
  CREAES: TipoSessao.PRONTUARIO_NASF,
  CASA_MAIS_AZUL: TipoSessao.PRONTUARIO_NASF,
  NAPE: TipoSessao.ANAMNESE_PSI,
  CRASF: TipoSessao.SINTESE,
};

function iniciais(nome: string): string {
  const partes = nome.trim().split(/\s+/);
  return `${partes[0]?.[0] ?? ""}${partes[partes.length - 1]?.[0] ?? ""}`.toUpperCase();
}

export function PerfilPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const usuario = useAuthStore((s) => s.usuario);
  const [encaminharAberto, setEncaminharAberto] = useState(false);

  const pessoaQuery = usePessoa(id);

  const registrarAtendimentoMutation = useMutation({
    mutationFn: () => {
      const sessionType = TIPO_SESSAO_POR_SERVICO[(usuario?.unidadeSigla ?? "NASF") as ServicoSigla];
      return criarAtendimento({
        personId: id!,
        unitId: usuario!.unidadeId,
        professionalId: usuario!.id,
        sessionType,
        dateTime: new Date().toISOString(),
      });
    },
    onSuccess: (atendimento) => {
      queryClient.invalidateQueries({ queryKey: ["atendimentos", "pessoa", id] });
      navigate(`/atendimentos/${atendimento.id}`);
    },
  });

  if (pessoaQuery.isLoading) return <LoadingState />;
  if (pessoaQuery.isError || !pessoaQuery.data) return <ErrorState onRetry={() => pessoaQuery.refetch()} />;

  const pessoa = pessoaQuery.data;
  const podeVerAuditoria = usuario?.papelRbac === PapelRbac.COORDENADOR || usuario?.papelRbac === PapelRbac.AUDITOR;

  return (
    <div>
      <div className="mb-6 flex flex-col justify-between gap-4 rounded-xl border border-border bg-white p-5 sm:flex-row sm:items-center">
        <div className="flex items-center gap-4">
          <Avatar className="size-14">
            <AvatarFallback className="bg-sus-blue-light text-lg text-sus-blue-dark">
              {pessoa.fullName ? iniciais(pessoa.fullName) : <User className="size-6" />}
            </AvatarFallback>
          </Avatar>
          <div>
            <h1 className="text-xl font-bold text-foreground">{pessoa.fullName}</h1>
            <p className="text-sm text-muted-foreground">
              {formatarIdade(pessoa.birthDate)} — nasc. {formatarData(pessoa.birthDate)}
              {pessoa.cns && ` — CNS ${pessoa.cns}`}
            </p>
            <div className="mt-2 flex flex-wrap gap-1.5">
              {(pessoa.services ?? []).map((servico) => (
                <ServiceChip key={servico} sigla={servico} />
              ))}
            </div>
          </div>
        </div>

        <div className="flex shrink-0 flex-wrap gap-2">
          <Button
            className="bg-sus-blue hover:bg-sus-blue-dark"
            onClick={() => registrarAtendimentoMutation.mutate()}
            disabled={registrarAtendimentoMutation.isPending}
          >
            <Stethoscope className="size-4" />
            Registrar atendimento
          </Button>
          <Button variant="outline" onClick={() => setEncaminharAberto(true)}>
            <Send className="size-4" />
            Criar encaminhamento
          </Button>
          <Button variant="outline" onClick={() => toast.info("Edicao de cadastro ainda nao disponivel neste prototipo.")}>
            <Pencil className="size-4" />
            Editar
          </Button>
        </div>
      </div>

      <Tabs defaultValue="timeline">
        <TabsList className="flex-wrap">
          <TabsTrigger value="timeline">Timeline</TabsTrigger>
          <TabsTrigger value="escolar">Escolar</TabsTrigger>
          <TabsTrigger value="familia">Familia</TabsTrigger>
          <TabsTrigger value="desenvolvimento">Desenvolvimento</TabsTrigger>
          <TabsTrigger value="clinico">Clinico</TabsTrigger>
          <TabsTrigger value="tratamentos-concomitantes">Tratamentos concomitantes</TabsTrigger>
          <TabsTrigger value="consentimentos">Consentimentos</TabsTrigger>
          <TabsTrigger value="atendimentos">Atendimentos</TabsTrigger>
          <TabsTrigger value="encaminhamentos">Encaminhamentos</TabsTrigger>
          <TabsTrigger value="fila">Fila</TabsTrigger>
          {podeVerAuditoria && <TabsTrigger value="auditoria">Auditoria</TabsTrigger>}
        </TabsList>

        <TabsContent value="timeline">
          <AbaTimeline pessoaId={pessoa.id} />
        </TabsContent>
        <TabsContent value="escolar">
          <AbaEscolar pessoa={pessoa} />
        </TabsContent>
        <TabsContent value="familia">
          <AbaFamilia pessoaId={pessoa.id} />
        </TabsContent>
        <TabsContent value="desenvolvimento">
          <AbaDesenvolvimento pessoaId={pessoa.id} />
        </TabsContent>
        <TabsContent value="clinico">
          <AbaClinico pessoaId={pessoa.id} />
        </TabsContent>
        <TabsContent value="tratamentos-concomitantes">
          <AbaTratamentosConcomitantes pessoaId={pessoa.id} />
        </TabsContent>
        <TabsContent value="consentimentos">
          <AbaConsentimentos pessoaId={pessoa.id} />
        </TabsContent>
        <TabsContent value="atendimentos">
          <AbaAtendimentos pessoaId={pessoa.id} />
        </TabsContent>
        <TabsContent value="encaminhamentos">
          <AbaEncaminhamentos pessoaId={pessoa.id} />
        </TabsContent>
        <TabsContent value="fila">
          <AbaFila pessoaId={pessoa.id} />
        </TabsContent>
        {podeVerAuditoria && (
          <TabsContent value="auditoria">
            <AbaAuditoria pessoaId={pessoa.id} />
          </TabsContent>
        )}
      </Tabs>

      <ReferralModal
        open={encaminharAberto}
        onOpenChange={setEncaminharAberto}
        pessoa={pessoa}
        onCreated={(encaminhamento) => navigate(`/encaminhamentos/${encaminhamento.id}/rastreio`)}
      />
    </div>
  );
}
