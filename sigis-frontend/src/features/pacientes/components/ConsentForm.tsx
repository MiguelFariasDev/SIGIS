import { Controller, type Control, type FieldErrors, type UseFormRegister } from "react-hook-form";
import { Checkbox } from "@/components/ui/checkbox";
import { Field, FieldError, FieldGroup, FieldLabel } from "@/components/ui/field";
import { Input } from "@/components/ui/input";
import type { CadastroPessoaFormValues } from "../schemas";

interface ConsentFormProps {
  register: UseFormRegister<CadastroPessoaFormValues>;
  control: Control<CadastroPessoaFormValues>;
  errors: FieldErrors<CadastroPessoaFormValues>;
}

/**
 * Secao 6 do cadastro unico (T04) — consentimentos LGPD (RNF02).
 * O consentimento clinico e obrigatorio; os demais sao independentes
 * e podem ser concedidos ou negados sem bloquear o cadastro.
 */
export function ConsentForm({ register, control, errors }: ConsentFormProps) {
  return (
    <FieldGroup>
      <div className="space-y-3 rounded-lg border border-border p-4">
        <label className="flex items-start gap-2 text-sm text-foreground">
          <Controller
            control={control}
            name="consentClinical"
            render={({ field }) => (
              <Checkbox checked={field.value} onCheckedChange={(v) => field.onChange(v === true)} />
            )}
          />
          <span>
            Autorizo o atendimento clinico e o registro dos dados de saude necessarios ao cuidado{" "}
            <span className="text-sus-red">*</span>
          </span>
        </label>
        <FieldError errors={errors.consentClinical ? [errors.consentClinical] : undefined} />

        <label className="flex items-start gap-2 text-sm text-foreground">
          <Controller
            control={control}
            name="consentEducational"
            render={({ field }) => (
              <Checkbox checked={field.value} onCheckedChange={(v) => field.onChange(v === true)} />
            )}
          />
          Autorizo o compartilhamento de dados com a Secretaria de Educacao
        </label>

        <label className="flex items-start gap-2 text-sm text-foreground">
          <Controller
            control={control}
            name="consentSocialAssistance"
            render={({ field }) => (
              <Checkbox checked={field.value} onCheckedChange={(v) => field.onChange(v === true)} />
            )}
          />
          Autorizo o compartilhamento de dados com a Assistencia Social
        </label>

        <label className="flex items-start gap-2 text-sm text-foreground">
          <Controller
            control={control}
            name="consentResearch"
            render={({ field }) => (
              <Checkbox checked={field.value} onCheckedChange={(v) => field.onChange(v === true)} />
            )}
          />
          Autorizo o uso de dados anonimizados para pesquisa
        </label>
      </div>

      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
        <Field>
          <FieldLabel htmlFor="consentCollectedAt">Data da coleta</FieldLabel>
          <Input id="consentCollectedAt" type="date" {...register("consentCollectedAt")} />
        </Field>
        <Field>
          <FieldLabel htmlFor="consentCollectedBy">Responsavel pela coleta</FieldLabel>
          <Input id="consentCollectedBy" {...register("consentCollectedBy")} />
        </Field>
      </div>
    </FieldGroup>
  );
}
