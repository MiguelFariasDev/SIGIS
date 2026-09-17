import type { FieldErrors, UseFormRegister } from "react-hook-form";
import { Field, FieldDescription, FieldGroup, FieldLegend, FieldLabel, FieldSet } from "@/components/ui/field";
import { Input } from "@/components/ui/input";
import type { CadastroPessoaFormValues } from "../schemas";

interface ResponsavelFormProps {
  register: UseFormRegister<CadastroPessoaFormValues>;
  errors: FieldErrors<CadastroPessoaFormValues>;
}

export function ResponsavelForm({ register }: ResponsavelFormProps) {
  return (
    <FieldSet>
      <FieldLegend>Responsavel</FieldLegend>
      <FieldDescription>Obrigatorio quando a pessoa cadastrada for menor de idade.</FieldDescription>
      <FieldGroup>
        <Field>
          <FieldLabel htmlFor="guardianName">Nome do responsavel</FieldLabel>
          <Input id="guardianName" {...register("guardianName")} />
        </Field>

        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
          <Field>
            <FieldLabel htmlFor="guardianCns">CNS do responsavel</FieldLabel>
            <Input id="guardianCns" {...register("guardianCns")} />
          </Field>
          <Field>
            <FieldLabel htmlFor="guardianBirthDate">Data de nascimento</FieldLabel>
            <Input id="guardianBirthDate" type="date" {...register("guardianBirthDate")} />
          </Field>
        </div>

        <Field>
          <FieldLabel htmlFor="guardianRelationship">Parentesco</FieldLabel>
          <Input id="guardianRelationship" placeholder="Mae, pai, avo(a)..." {...register("guardianRelationship")} />
        </Field>
      </FieldGroup>
    </FieldSet>
  );
}
