import { Controller, type Control, type UseFormRegister } from "react-hook-form";
import { Field, FieldGroup, FieldLabel } from "@/components/ui/field";
import { Input } from "@/components/ui/input";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Textarea } from "@/components/ui/textarea";
import { unidadesMock } from "@/lib/mocks/data/unidades.mock";
import { SecretariaResponsavel } from "@/lib/types/enums";
import type { CadastroPessoaFormValues } from "../schemas";

const UNIDADES_SAUDE = unidadesMock.filter((u) => u.secretariaResponsavel === SecretariaResponsavel.SAUDE);

interface ClinicalProfileFormProps {
  register: UseFormRegister<CadastroPessoaFormValues>;
  control: Control<CadastroPessoaFormValues>;
}

/** Secao 3 do cadastro unico (T04) — perfil clinico NASF. */
export function ClinicalProfileForm({ register, control }: ClinicalProfileFormProps) {
  return (
    <FieldGroup>
      <Field>
        <FieldLabel htmlFor="medicalRecordNumber">Numero do prontuario</FieldLabel>
        <Input id="medicalRecordNumber" {...register("medicalRecordNumber")} />
      </Field>

      <Field>
        <FieldLabel htmlFor="clinicalHypothesis">Hipotese diagnostica</FieldLabel>
        <Textarea id="clinicalHypothesis" rows={3} {...register("clinicalHypothesis")} />
      </Field>

      <Field>
        <FieldLabel htmlFor="apsReferenceUnitId">Unidade de referencia (APS)</FieldLabel>
        <Controller
          control={control}
          name="apsReferenceUnitId"
          render={({ field }) => (
            <Select value={field.value ?? ""} onValueChange={field.onChange}>
              <SelectTrigger id="apsReferenceUnitId" className="w-full">
                <SelectValue placeholder="Selecione a unidade" />
              </SelectTrigger>
              <SelectContent>
                {UNIDADES_SAUDE.map((unidade) => (
                  <SelectItem key={unidade.id} value={unidade.id}>
                    {unidade.nome} ({unidade.sigla})
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          )}
        />
      </Field>
    </FieldGroup>
  );
}
