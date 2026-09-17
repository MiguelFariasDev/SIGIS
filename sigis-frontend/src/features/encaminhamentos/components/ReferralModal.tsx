import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Lock, Send } from "lucide-react";
import { useMemo, useState } from "react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { Dialog, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Field, FieldGroup, FieldLabel } from "@/components/ui/field";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Textarea } from "@/components/ui/textarea";
import { fetchConsentimentos } from "@/features/pacientes/api";
import { GrantConsentModal } from "@/features/pacientes/components/GrantConsentModal";
import { fetchLegalBasis } from "@/lib/api/legalBasis";
import { unidadesMock } from "@/lib/mocks/data/unidades.mock";
import { PrioridadeFila } from "@/lib/types/enums";
import type { Encaminhamento } from "@/lib/types/encaminhamento";
import type { Secretariat } from "@/lib/types/legalBasis";
import type { ConsentType } from "@/lib/types/personConsent";
import type { Person } from "@/lib/types/person";
import { PRIORIDADE_LABEL, SECRETARIAT_LABEL, type ServicoSigla, SERVICO_SECRETARIAT } from "@/lib/utils/constants";
import { useAuthStore } from "@/stores/authStore";
import { criarEncaminhamento } from "../api";

interface ReferralModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  pessoa: Person;
  onCreated: (encaminhamento: Encaminhamento) => void;
}

const CONSENT_TYPE_POR_SECRETARIA: Record<Secretariat, ConsentType> = {
  Health: "Clinical",
  Education: "Educational",
  SocialAssistance: "SocialAssistance",
};

/**
 * T16 — Encaminhar paciente. Quando origem e destino pertencem a
 * secretarias diferentes (RF12/RF13), exige consentimento ja concedido
 * para o tipo correspondente e base legal — nunca encaminha "silencioso".
 */
export function ReferralModal({ open, onOpenChange, pessoa, onCreated }: ReferralModalProps) {
  const usuario = useAuthStore((s) => s.usuario);
  const queryClient = useQueryClient();
  const [unidadeDestinoId, setUnidadeDestinoId] = useState("");
  const [motivo, setMotivo] = useState("");
  const [prioridade, setPrioridade] = useState<PrioridadeFila>(PrioridadeFila.CURTO_PRAZO);
  const [legalBasisId, setLegalBasisId] = useState("");
  const [consentModalAberto, setConsentModalAberto] = useState(false);

  const legalBasisQuery = useQuery({ queryKey: ["legal-basis"], queryFn: fetchLegalBasis, enabled: open });
  const consentimentosQuery = useQuery({
    queryKey: ["pessoas", pessoa.id, "consentimentos"],
    queryFn: () => fetchConsentimentos(pessoa.id),
    enabled: open,
  });

  const origemSecretariat = usuario ? SERVICO_SECRETARIAT[usuario.unidadeSigla as ServicoSigla] : undefined;
  const unidadeDestino = unidadesMock.find((u) => u.id === unidadeDestinoId);
  const destinoSecretariat = unidadeDestino ? SERVICO_SECRETARIAT[unidadeDestino.sigla as ServicoSigla] : undefined;
  const crossSecretaria = !!origemSecretariat && !!destinoSecretariat && origemSecretariat !== destinoSecretariat;

  const tipoConsentimentoNecessario = destinoSecretariat ? CONSENT_TYPE_POR_SECRETARIA[destinoSecretariat] : undefined;
  const temConsentimento = useMemo(
    () =>
      !tipoConsentimentoNecessario ||
      (consentimentosQuery.data ?? []).some((c) => c.type === tipoConsentimentoNecessario && c.granted),
    [consentimentosQuery.data, tipoConsentimentoNecessario],
  );

  const bloqueadoPorConsentimento = crossSecretaria && !temConsentimento;
  const motivoValido = motivo.trim().length >= 50;
  const podeEnviar =
    !!unidadeDestinoId && motivoValido && !bloqueadoPorConsentimento && (!crossSecretaria || !!legalBasisId);

  const mutation = useMutation({
    mutationFn: () =>
      criarEncaminhamento({
        personId: pessoa.id,
        unidadeOrigemId: usuario!.unidadeId,
        unidadeDestinoId,
        motivo,
        prioridade,
        legalBasisId: crossSecretaria ? legalBasisId : undefined,
      }),
    onSuccess: (encaminhamento) => {
      queryClient.invalidateQueries({ queryKey: ["encaminhamentos"] });
      toast.success("Encaminhamento registrado com sucesso.");
      setUnidadeDestinoId("");
      setMotivo("");
      setLegalBasisId("");
      onOpenChange(false);
      onCreated(encaminhamento);
    },
    onError: () => toast.error("Nao foi possivel registrar o encaminhamento."),
  });

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Encaminhar {pessoa.fullName}</DialogTitle>
          <DialogDescription>Encaminhe para outra unidade da rede, com motivo e prioridade.</DialogDescription>
        </DialogHeader>

        <FieldGroup>
          <Field>
            <FieldLabel htmlFor="unidadeDestinoId">Unidade de destino</FieldLabel>
            <Select value={unidadeDestinoId} onValueChange={setUnidadeDestinoId}>
              <SelectTrigger id="unidadeDestinoId" className="w-full">
                <SelectValue placeholder="Selecione a unidade" />
              </SelectTrigger>
              <SelectContent>
                {unidadesMock
                  .filter((u) => u.id !== usuario?.unidadeId)
                  .map((u) => (
                    <SelectItem key={u.id} value={u.id}>
                      {u.nome} ({u.sigla})
                    </SelectItem>
                  ))}
              </SelectContent>
            </Select>
          </Field>

          {crossSecretaria && destinoSecretariat && (
            <div className="rounded-lg border border-sus-yellow/40 bg-sus-yellow-light p-3 text-xs text-sus-yellow-dark">
              Este encaminhamento e cross-secretaria (para {SECRETARIAT_LABEL[destinoSecretariat]}) e sera registrado em
              auditoria (RF13).
            </div>
          )}

          {bloqueadoPorConsentimento && tipoConsentimentoNecessario && (
            <div className="flex flex-col gap-2 rounded-lg border border-sus-red/30 bg-sus-red-light p-3 text-xs text-sus-red-dark">
              <span className="flex items-center gap-1.5 font-medium">
                <Lock className="size-3.5" />
                Nao ha consentimento concedido para {SECRETARIAT_LABEL[destinoSecretariat!]}. O encaminhamento esta
                bloqueado ate o consentimento ser registrado.
              </span>
              <Button size="sm" variant="outline" className="w-fit" onClick={() => setConsentModalAberto(true)}>
                Registrar consentimento
              </Button>
            </div>
          )}

          {crossSecretaria && !bloqueadoPorConsentimento && (
            <Field>
              <FieldLabel htmlFor="legalBasisId">Base legal</FieldLabel>
              <Select value={legalBasisId} onValueChange={setLegalBasisId}>
                <SelectTrigger id="legalBasisId" className="w-full">
                  <SelectValue placeholder="Selecione a base legal" />
                </SelectTrigger>
                <SelectContent>
                  {legalBasisQuery.data?.map((base) => (
                    <SelectItem key={base.id} value={base.id}>
                      {base.article} — {base.description}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </Field>
          )}

          <Field>
            <FieldLabel htmlFor="prioridade">Prioridade</FieldLabel>
            <Select value={prioridade} onValueChange={(v) => setPrioridade(v as PrioridadeFila)}>
              <SelectTrigger id="prioridade" className="w-full">
                <SelectValue />
              </SelectTrigger>
              <SelectContent>
                {Object.values(PrioridadeFila).map((p) => (
                  <SelectItem key={p} value={p}>
                    {PRIORIDADE_LABEL[p]}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </Field>

          <Field>
            <FieldLabel htmlFor="motivo">Motivo do encaminhamento (minimo 50 caracteres)</FieldLabel>
            <Textarea id="motivo" rows={4} value={motivo} onChange={(e) => setMotivo(e.target.value)} />
            {!motivoValido && motivo.length > 0 && (
              <p className="text-xs text-sus-red">Faltam {50 - motivo.trim().length} caracteres.</p>
            )}
          </Field>
        </FieldGroup>

        <DialogFooter>
          <Button
            className="bg-sus-blue hover:bg-sus-blue-dark"
            onClick={() => mutation.mutate()}
            disabled={!podeEnviar || mutation.isPending}
          >
            <Send className="size-4" />
            {mutation.isPending ? "Enviando..." : "Encaminhar"}
          </Button>
        </DialogFooter>
      </DialogContent>

      <GrantConsentModal open={consentModalAberto} onOpenChange={setConsentModalAberto} personId={pessoa.id} />
    </Dialog>
  );
}
