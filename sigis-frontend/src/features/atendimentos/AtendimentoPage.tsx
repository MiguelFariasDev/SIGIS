import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { AlertTriangle, Save } from "lucide-react";
import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { toast } from "sonner";
import { ErrorState } from "@/components/common/ErrorState";
import { LoadingState } from "@/components/common/LoadingState";
import { PageHeader } from "@/components/common/PageHeader";
import { ServiceChip } from "@/components/domain/ServiceChip";
import { Button } from "@/components/ui/button";
import { Checkbox } from "@/components/ui/checkbox";
import { Field, FieldLabel } from "@/components/ui/field";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Textarea } from "@/components/ui/textarea";
import { fetchFilaPorPessoa } from "@/features/filas/api";
import { profissionaisMock } from "@/lib/mocks/data/profissionais.mock";
import { StatusComparecimento, StatusFila } from "@/lib/types/enums";
import { TIPO_SESSAO_LABEL } from "@/lib/utils/constants";
import { SESSION_SCHEMAS } from "@/templates/sessionSchemas";
import { atualizarAtendimento, fetchAtendimento, fetchAtendimentosPorPessoa } from "./api";
import { ComparecimentoToggle } from "./components/ComparecimentoToggle";
import { DynamicForm } from "./components/DynamicForm";

const ENCAMINHAMENTOS_INTERNOS = [
  "Psicologo",
  "Nutricionista",
  "Fonoaudiologo",
  "Terapeuta Ocupacional",
  "Assistente Social",
];

export function AtendimentoPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const queryClient = useQueryClient();

  const [attendanceStatus, setAttendanceStatus] = useState<StatusComparecimento>(StatusComparecimento.AGENDADO);
  const [dadosFormulario, setDadosFormulario] = useState<Record<string, unknown>>({});
  const [triagedByProfessionalId, setTriagedByProfessionalId] = useState<string | undefined>(undefined);
  const [mainComplaint, setMainComplaint] = useState("");
  const [internalReferrals, setInternalReferrals] = useState<string[]>([]);

  const atendimentoQuery = useQuery({
    queryKey: ["atendimentos", id],
    queryFn: () => fetchAtendimento(id!),
    enabled: !!id,
  });

  const historicoQuery = useQuery({
    queryKey: ["atendimentos", "pessoa", atendimentoQuery.data?.personId],
    queryFn: () => fetchAtendimentosPorPessoa(atendimentoQuery.data!.personId),
    enabled: !!atendimentoQuery.data,
  });

  const filaQuery = useQuery({
    queryKey: ["filas", "pessoa", atendimentoQuery.data?.personId],
    queryFn: () => fetchFilaPorPessoa(atendimentoQuery.data!.personId),
    enabled: !!atendimentoQuery.data,
  });

  useEffect(() => {
    if (atendimentoQuery.data) {
      setAttendanceStatus(atendimentoQuery.data.attendanceStatus);
      setDadosFormulario(atendimentoQuery.data.dadosFormulario);
      setTriagedByProfessionalId(atendimentoQuery.data.triagedByProfessionalId);
      setMainComplaint(atendimentoQuery.data.mainComplaint ?? "");
      setInternalReferrals(atendimentoQuery.data.internalReferrals ?? []);
    }
  }, [atendimentoQuery.data]);

  const salvarMutation = useMutation({
    mutationFn: () =>
      atualizarAtendimento(id!, {
        attendanceStatus,
        dadosFormulario,
        triagedByProfessionalId,
        mainComplaint: mainComplaint || undefined,
        internalReferrals,
      }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["atendimentos"] });
      toast.success("Sessao registrada com sucesso.");
      navigate(`/pacientes/${atendimentoQuery.data?.personId}`);
    },
  });

  if (atendimentoQuery.isLoading) return <LoadingState />;
  if (atendimentoQuery.isError || !atendimentoQuery.data) return <ErrorState onRetry={() => atendimentoQuery.refetch()} />;

  const atendimento = atendimentoQuery.data;
  const schema = SESSION_SCHEMAS[atendimento.sessionType];

  const faltasAnteriores =
    historicoQuery.data?.filter((a) => a.id !== atendimento.id && a.attendanceStatus === StatusComparecimento.FALTOU)
      .length ?? 0;

  const atendimentoAtivoEmOutraUnidade = filaQuery.data?.find(
    (f) =>
      f.unidadeId !== atendimento.unitId &&
      (f.status === StatusFila.AGUARDANDO || f.status === StatusFila.EM_ATENDIMENTO),
  );

  function toggleEncaminhamento(nome: string, marcado: boolean) {
    setInternalReferrals((atual) => (marcado ? [...atual, nome] : atual.filter((v) => v !== nome)));
  }

  return (
    <div className="mx-auto max-w-2xl">
      <PageHeader
        title={atendimento.personName}
        description={`${TIPO_SESSAO_LABEL[atendimento.sessionType]} — Sessao ${atendimento.sessionNumber}`}
        actions={<ServiceChip sigla={atendimento.unitCode} />}
      />

      {atendimentoAtivoEmOutraUnidade && (
        <div className="mb-4 flex gap-3 rounded-xl border border-sus-yellow/40 bg-sus-yellow-light p-4">
          <AlertTriangle className="size-5 shrink-0 text-sus-yellow-dark" />
          <p className="text-sm text-sus-yellow-dark">
            Este paciente ja possui atendimento ativo em <strong>{atendimentoAtivoEmOutraUnidade.especialidade}</strong>{" "}
            em outra unidade da rede. Verifique a linha do tempo antes de prosseguir para evitar duplicidade de
            atendimento.
          </p>
        </div>
      )}

      {faltasAnteriores >= 1 && attendanceStatus !== StatusComparecimento.COMPARECEU && (
        <div className="mb-4 flex gap-3 rounded-xl border border-sus-red/30 bg-sus-red-light p-4">
          <AlertTriangle className="size-5 shrink-0 text-sus-red" />
          <p className="text-sm text-sus-red-dark">
            Este paciente ja possui {faltasAnteriores} falta registrada. Uma nova falta aciona o fluxo de{" "}
            <strong>busca ativa</strong> (RN04).
          </p>
        </div>
      )}

      <div className="mb-6">
        <p className="mb-2 text-sm font-medium text-foreground">Comparecimento</p>
        <ComparecimentoToggle value={attendanceStatus} onChange={setAttendanceStatus} />
      </div>

      <div className="mb-6 space-y-4 rounded-xl border border-border bg-white p-5">
        <p className="text-sm font-semibold text-foreground">Triagem</p>

        <Field>
          <FieldLabel htmlFor="triagedBy">Triagem realizada por</FieldLabel>
          <Select value={triagedByProfessionalId ?? ""} onValueChange={setTriagedByProfessionalId}>
            <SelectTrigger id="triagedBy" className="w-full">
              <SelectValue placeholder="Selecione o profissional" />
            </SelectTrigger>
            <SelectContent>
              {profissionaisMock.map((prof) => (
                <SelectItem key={prof.id} value={prof.id}>
                  {prof.nome} — {prof.especialidade}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
        </Field>

        <Field>
          <FieldLabel htmlFor="mainComplaint">Queixa principal</FieldLabel>
          <Textarea
            id="mainComplaint"
            rows={3}
            value={mainComplaint}
            onChange={(e) => setMainComplaint(e.target.value)}
          />
        </Field>

        <Field>
          <FieldLabel>Encaminhamentos internos</FieldLabel>
          <div className="flex flex-wrap gap-3">
            {ENCAMINHAMENTOS_INTERNOS.map((nome) => (
              <label key={nome} className="flex items-center gap-1.5 text-sm text-foreground">
                <Checkbox
                  checked={internalReferrals.includes(nome)}
                  onCheckedChange={(v) => toggleEncaminhamento(nome, v === true)}
                />
                {nome}
              </label>
            ))}
          </div>
        </Field>
      </div>

      {attendanceStatus === StatusComparecimento.COMPARECEU && (
        <div className="mb-6 rounded-xl border border-border bg-white p-5">
          <DynamicForm schema={schema} values={dadosFormulario} onChange={setDadosFormulario} />
        </div>
      )}

      <div className="flex justify-end">
        <Button
          className="bg-sus-blue hover:bg-sus-blue-dark"
          onClick={() => salvarMutation.mutate()}
          disabled={salvarMutation.isPending}
        >
          <Save className="size-4" />
          {salvarMutation.isPending ? "Salvando..." : "Salvar sessao"}
        </Button>
      </div>
    </div>
  );
}
