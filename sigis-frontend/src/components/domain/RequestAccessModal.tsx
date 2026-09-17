import { zodResolver } from "@hookform/resolvers/zod";
import { useMutation, useQuery } from "@tanstack/react-query";
import { ShieldCheck } from "lucide-react";
import { Controller, useForm } from "react-hook-form";
import { toast } from "sonner";
import { z } from "zod";
import { Button } from "@/components/ui/button";
import { Checkbox } from "@/components/ui/checkbox";
import { Dialog, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Field, FieldError, FieldGroup, FieldLabel } from "@/components/ui/field";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Textarea } from "@/components/ui/textarea";
import { fetchLegalBasis } from "@/lib/api/legalBasis";
import { solicitarAcesso } from "@/features/pacientes/api";
import type { Secretariat } from "@/lib/types/legalBasis";
import { SECRETARIAT_LABEL } from "@/lib/utils/constants";

const schema = z.object({
  legalBasisId: z.string().min(1, "Selecione a base legal."),
  purpose: z.string().min(1, "Selecione a finalidade."),
  justification: z.string().min(50, "A justificativa deve ter pelo menos 50 caracteres."),
  confirmado: z.boolean().refine((v) => v === true, { message: "Confirme para prosseguir." }),
});

type FormValues = z.infer<typeof schema>;

const FINALIDADES = [
  "Continuidade do cuidado",
  "Avaliacao multiprofissional",
  "Revisao de encaminhamento",
  "Auditoria e conformidade",
];

interface RequestAccessModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  personId: string;
  secretariat: Secretariat;
  onGranted: (secretariat: Secretariat) => void;
}

/**
 * T15 — Solicitacao de acesso a evento cross-secretaria (RF12).
 * Toda concessao fica registrada em log de auditoria com base legal e
 * justificativa (RF13) — nunca ha acesso "silencioso".
 */
export function RequestAccessModal({ open, onOpenChange, personId, secretariat, onGranted }: RequestAccessModalProps) {
  const legalBasisQuery = useQuery({ queryKey: ["legal-basis"], queryFn: fetchLegalBasis, enabled: open });

  const {
    control,
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<FormValues>({
    resolver: zodResolver(schema),
    defaultValues: { legalBasisId: "", purpose: "", justification: "", confirmado: false },
  });

  const mutation = useMutation({
    mutationFn: (values: FormValues) =>
      solicitarAcesso(personId, {
        legalBasisId: values.legalBasisId,
        purpose: values.purpose,
        justification: values.justification,
      }),
    onSuccess: () => {
      toast.success("Acesso concedido e registrado em auditoria.");
      onGranted(secretariat);
      reset();
      onOpenChange(false);
    },
    onError: () => {
      toast.error("Nao foi possivel registrar a solicitacao.");
    },
  });

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Solicitar acesso ao conteudo</DialogTitle>
          <DialogDescription>
            Este evento pertence a secretaria de {SECRETARIAT_LABEL[secretariat]} e esta fora do seu escopo padrao.
            O acesso sera registrado em log de auditoria com a base legal e a justificativa informadas (RF12/RF13).
          </DialogDescription>
        </DialogHeader>

        <form
          id="request-access-form"
          onSubmit={handleSubmit((values) => mutation.mutate(values))}
          noValidate
        >
          <FieldGroup>
            <Field data-invalid={!!errors.legalBasisId}>
              <FieldLabel htmlFor="legalBasisId">Base legal</FieldLabel>
              <Controller
                control={control}
                name="legalBasisId"
                render={({ field }) => (
                  <Select value={field.value} onValueChange={field.onChange}>
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
                )}
              />
              <FieldError errors={errors.legalBasisId ? [errors.legalBasisId] : undefined} />
            </Field>

            <Field data-invalid={!!errors.purpose}>
              <FieldLabel htmlFor="purpose">Finalidade</FieldLabel>
              <Controller
                control={control}
                name="purpose"
                render={({ field }) => (
                  <Select value={field.value} onValueChange={field.onChange}>
                    <SelectTrigger id="purpose" className="w-full">
                      <SelectValue placeholder="Selecione a finalidade" />
                    </SelectTrigger>
                    <SelectContent>
                      {FINALIDADES.map((finalidade) => (
                        <SelectItem key={finalidade} value={finalidade}>
                          {finalidade}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                )}
              />
              <FieldError errors={errors.purpose ? [errors.purpose] : undefined} />
            </Field>

            <Field data-invalid={!!errors.justification}>
              <FieldLabel htmlFor="justification">Justificativa (minimo 50 caracteres)</FieldLabel>
              <Textarea id="justification" rows={4} {...register("justification")} />
              <FieldError errors={errors.justification ? [errors.justification] : undefined} />
            </Field>

            <label className="flex items-start gap-2 text-sm text-foreground">
              <Controller
                control={control}
                name="confirmado"
                render={({ field }) => (
                  <Checkbox checked={field.value} onCheckedChange={(v) => field.onChange(v === true)} />
                )}
              />
              Confirmo que este acesso e necessario para o cuidado da pessoa e sera registrado em auditoria.
            </label>
            <FieldError errors={errors.confirmado ? [errors.confirmado] : undefined} />
          </FieldGroup>
        </form>

        <DialogFooter>
          <Button type="submit" form="request-access-form" className="bg-sus-blue hover:bg-sus-blue-dark" disabled={mutation.isPending}>
            <ShieldCheck className="size-4" />
            {mutation.isPending ? "Enviando..." : "Solicitar acesso"}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
