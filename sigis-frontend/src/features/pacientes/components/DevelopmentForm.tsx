import { Controller, type Control, type UseFormRegister } from "react-hook-form";
import { Checkbox } from "@/components/ui/checkbox";
import { Field, FieldGroup, FieldLabel } from "@/components/ui/field";
import { Input } from "@/components/ui/input";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Textarea } from "@/components/ui/textarea";
import type { CadastroPessoaFormValues } from "../schemas";

interface DevelopmentFormProps {
  register: UseFormRegister<CadastroPessoaFormValues>;
  control: Control<CadastroPessoaFormValues>;
}

/** Secao 5 do cadastro unico (T04) — marcos de desenvolvimento. */
export function DevelopmentForm({ register, control }: DevelopmentFormProps) {
  return (
    <FieldGroup>
      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
        <Field>
          <FieldLabel htmlFor="ageWalkedMonths">Idade em que comecou a andar (meses)</FieldLabel>
          <Input id="ageWalkedMonths" type="number" min={0} {...register("ageWalkedMonths")} />
        </Field>
        <Field>
          <FieldLabel htmlFor="ageTalkedMonths">Idade em que comecou a falar (meses)</FieldLabel>
          <Input id="ageTalkedMonths" type="number" min={0} {...register("ageTalkedMonths")} />
        </Field>
      </div>

      <div className="grid grid-cols-1 gap-3 sm:grid-cols-2">
        {(
          [
            ["locomotionDifficulty", "Dificuldade de locomocao"],
            ["coordinationDifficulty", "Dificuldade de coordenacao"],
            ["visualDifficulty", "Dificuldade visual"],
            ["hearingDifficulty", "Dificuldade auditiva"],
          ] as const
        ).map(([campo, label]) => (
          <label key={campo} className="flex items-center gap-2 text-sm text-foreground">
            <Controller
              control={control}
              name={campo}
              render={({ field }) => (
                <Checkbox checked={field.value} onCheckedChange={(v) => field.onChange(v === true)} />
              )}
            />
            {label}
          </label>
        ))}
      </div>

      <Field>
        <FieldLabel htmlFor="speechProblems">Problemas de fala</FieldLabel>
        <Textarea id="speechProblems" rows={2} {...register("speechProblems")} />
      </Field>

      <Field>
        <FieldLabel htmlFor="commandComprehension">Compreensao de comandos</FieldLabel>
        <Textarea id="commandComprehension" rows={2} {...register("commandComprehension")} />
      </Field>

      <Field>
        <FieldLabel htmlFor="manualDominance">Dominancia manual</FieldLabel>
        <Controller
          control={control}
          name="manualDominance"
          render={({ field }) => (
            <Select value={field.value ?? ""} onValueChange={field.onChange}>
              <SelectTrigger id="manualDominance" className="w-full">
                <SelectValue placeholder="Selecione" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="Destro">Destro</SelectItem>
                <SelectItem value="Canhoto">Canhoto</SelectItem>
                <SelectItem value="Ambidestro">Ambidestro</SelectItem>
              </SelectContent>
            </Select>
          )}
        />
      </Field>
    </FieldGroup>
  );
}
