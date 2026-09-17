import { Controller, type Control, type UseFormRegister } from "react-hook-form";
import { Checkbox } from "@/components/ui/checkbox";
import { Field, FieldGroup, FieldLabel } from "@/components/ui/field";
import { Input } from "@/components/ui/input";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Textarea } from "@/components/ui/textarea";
import type { CadastroPessoaFormValues } from "../schemas";

interface FamilyCompositionFormProps {
  register: UseFormRegister<CadastroPessoaFormValues>;
  control: Control<CadastroPessoaFormValues>;
}

/** Secao 4 do cadastro unico (T04) — composicao familiar. */
export function FamilyCompositionForm({ register, control }: FamilyCompositionFormProps) {
  return (
    <FieldGroup>
      <div className="grid grid-cols-1 gap-4 sm:grid-cols-3">
        <Field>
          <FieldLabel htmlFor="fatherName">Nome do pai</FieldLabel>
          <Input id="fatherName" {...register("fatherName")} />
        </Field>
        <Field>
          <FieldLabel htmlFor="fatherEducation">Escolaridade do pai</FieldLabel>
          <Input id="fatherEducation" {...register("fatherEducation")} />
        </Field>
        <Field>
          <FieldLabel htmlFor="fatherOccupation">Ocupacao do pai</FieldLabel>
          <Input id="fatherOccupation" {...register("fatherOccupation")} />
        </Field>
      </div>

      <div className="grid grid-cols-1 gap-4 sm:grid-cols-3">
        <Field>
          <FieldLabel htmlFor="familyMotherName">Nome da mae</FieldLabel>
          <Input id="familyMotherName" {...register("familyMotherName")} />
        </Field>
        <Field>
          <FieldLabel htmlFor="motherEducation">Escolaridade da mae</FieldLabel>
          <Input id="motherEducation" {...register("motherEducation")} />
        </Field>
        <Field>
          <FieldLabel htmlFor="motherOccupation">Ocupacao da mae</FieldLabel>
          <Input id="motherOccupation" {...register("motherOccupation")} />
        </Field>
      </div>

      <div className="grid grid-cols-1 gap-4 sm:grid-cols-3">
        <Field>
          <FieldLabel htmlFor="siblingsCount">Numero de irmaos</FieldLabel>
          <Input id="siblingsCount" type="number" min={0} {...register("siblingsCount")} />
        </Field>
        <Field>
          <FieldLabel htmlFor="siblingsAges">Idades dos irmaos</FieldLabel>
          <Input id="siblingsAges" placeholder="Ex.: 8, 12" {...register("siblingsAges")} />
        </Field>
        <Field>
          <FieldLabel htmlFor="householdMembersCount">Pessoas no domicilio</FieldLabel>
          <Input id="householdMembersCount" type="number" min={1} {...register("householdMembersCount")} />
        </Field>
      </div>

      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
        <Field>
          <FieldLabel htmlFor="parentsMaritalStatus">Estado civil dos pais</FieldLabel>
          <Input id="parentsMaritalStatus" {...register("parentsMaritalStatus")} />
        </Field>
        <Field>
          <FieldLabel htmlFor="filiationType">Tipo de filiacao</FieldLabel>
          <Controller
            control={control}
            name="filiationType"
            render={({ field }) => (
              <Select value={field.value ?? ""} onValueChange={field.onChange}>
                <SelectTrigger id="filiationType" className="w-full">
                  <SelectValue placeholder="Selecione" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="Natural">Natural</SelectItem>
                  <SelectItem value="Adotivo">Adotivo</SelectItem>
                </SelectContent>
              </Select>
            )}
          />
        </Field>
      </div>

      <label className="flex items-center gap-2 text-sm text-foreground">
        <Controller
          control={control}
          name="plannedPregnancy"
          render={({ field }) => <Checkbox checked={field.value} onCheckedChange={(v) => field.onChange(v === true)} />}
        />
        Gravidez planejada
      </label>

      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
        <Field>
          <FieldLabel htmlFor="pregnanciesCount">Numero de gestacoes</FieldLabel>
          <Input id="pregnanciesCount" type="number" min={0} {...register("pregnanciesCount")} />
        </Field>
        <Field>
          <FieldLabel htmlFor="abortionsCount">Numero de abortos</FieldLabel>
          <Input id="abortionsCount" type="number" min={0} {...register("abortionsCount")} />
        </Field>
      </div>

      <Field>
        <FieldLabel htmlFor="pregnancyHealthIssue">Intercorrencias na gestacao</FieldLabel>
        <Textarea id="pregnancyHealthIssue" rows={2} {...register("pregnancyHealthIssue")} />
      </Field>

      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
        <Field>
          <FieldLabel htmlFor="deliveryType">Tipo de parto</FieldLabel>
          <Controller
            control={control}
            name="deliveryType"
            render={({ field }) => (
              <Select value={field.value ?? ""} onValueChange={field.onChange}>
                <SelectTrigger id="deliveryType" className="w-full">
                  <SelectValue placeholder="Selecione" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="Normal">Normal</SelectItem>
                  <SelectItem value="Cesarea">Cesarea</SelectItem>
                </SelectContent>
              </Select>
            )}
          />
        </Field>
        <Field>
          <FieldLabel htmlFor="medicationDuringPregnancy">Medicacao na gestacao</FieldLabel>
          <Input id="medicationDuringPregnancy" {...register("medicationDuringPregnancy")} />
        </Field>
      </div>
    </FieldGroup>
  );
}
