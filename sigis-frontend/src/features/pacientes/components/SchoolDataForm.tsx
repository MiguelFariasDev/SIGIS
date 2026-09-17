import { Controller, type Control, type UseFormRegister } from "react-hook-form";
import { Checkbox } from "@/components/ui/checkbox";
import { Field, FieldGroup, FieldLabel } from "@/components/ui/field";
import { Input } from "@/components/ui/input";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import type { CadastroPessoaFormValues } from "../schemas";

const OPCOES_DEFICIENCIA = [
  "TEA",
  "TDAH",
  "Deficiencia Intelectual",
  "Deficiencia Auditiva",
  "Deficiencia Visual",
  "Deficiencia Fisica",
  "Outro",
];

interface SchoolDataFormProps {
  register: UseFormRegister<CadastroPessoaFormValues>;
  control: Control<CadastroPessoaFormValues>;
}

/**
 * Secao 2 do cadastro unico (T04) — faceta Educacao. Todos os campos
 * sao opcionais: uma pessoa pode nao ter vinculo escolar no momento
 * do cadastro (por exemplo, encaminhada apenas pela Saude).
 */
export function SchoolDataForm({ register, control }: SchoolDataFormProps) {
  return (
    <FieldGroup>
      <Field>
        <FieldLabel htmlFor="naturality">Naturalidade</FieldLabel>
        <Input id="naturality" {...register("naturality")} />
      </Field>

      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
        <Field>
          <FieldLabel htmlFor="currentSchool">Escola atual</FieldLabel>
          <Input id="currentSchool" {...register("currentSchool")} />
        </Field>
        <Field>
          <FieldLabel htmlFor="grade">Serie/ano</FieldLabel>
          <Input id="grade" {...register("grade")} />
        </Field>
      </div>

      <div className="grid grid-cols-1 gap-4 sm:grid-cols-3">
        <Field>
          <FieldLabel htmlFor="shift">Turno</FieldLabel>
          <Controller
            control={control}
            name="shift"
            render={({ field }) => (
              <Select value={field.value ?? ""} onValueChange={field.onChange}>
                <SelectTrigger id="shift" className="w-full">
                  <SelectValue placeholder="Selecione" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="Manha">Manha</SelectItem>
                  <SelectItem value="Tarde">Tarde</SelectItem>
                  <SelectItem value="Noite">Noite</SelectItem>
                  <SelectItem value="Integral">Integral</SelectItem>
                </SelectContent>
              </Select>
            )}
          />
        </Field>
        <Field>
          <FieldLabel htmlFor="classGroup">Turma</FieldLabel>
          <Input id="classGroup" {...register("classGroup")} />
        </Field>
        <Field>
          <FieldLabel htmlFor="zone">Zona</FieldLabel>
          <Controller
            control={control}
            name="zone"
            render={({ field }) => (
              <Select value={field.value ?? ""} onValueChange={field.onChange}>
                <SelectTrigger id="zone" className="w-full">
                  <SelectValue placeholder="Selecione" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="Urbana">Urbana</SelectItem>
                  <SelectItem value="Rural">Rural</SelectItem>
                </SelectContent>
              </Select>
            )}
          />
        </Field>
      </div>

      <Field>
        <FieldLabel htmlFor="schoolEnrollment">Matricula escolar</FieldLabel>
        <Input id="schoolEnrollment" {...register("schoolEnrollment")} />
      </Field>

      <div className="grid grid-cols-1 gap-3 sm:grid-cols-2">
        {(
          [
            ["referredBySchool", "Encaminhado pela escola"],
            ["needsSpecialEducation", "Necessita de educacao especial"],
            ["attendsTutoring", "Frequenta reforco escolar"],
            ["hasFailedGrade", "Ja reprovou de ano"],
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
        <FieldLabel>Tipos de deficiencia/condicao</FieldLabel>
        <Controller
          control={control}
          name="disabilityTypes"
          render={({ field }) => (
            <div className="flex flex-wrap gap-3">
              {OPCOES_DEFICIENCIA.map((opcao) => {
                const marcado = field.value?.includes(opcao) ?? false;
                return (
                  <label key={opcao} className="flex items-center gap-1.5 text-sm text-foreground">
                    <Checkbox
                      checked={marcado}
                      onCheckedChange={(checked) => {
                        const atual = field.value ?? [];
                        field.onChange(checked ? [...atual, opcao] : atual.filter((v: string) => v !== opcao));
                      }}
                    />
                    {opcao}
                  </label>
                );
              })}
            </div>
          )}
        />
      </Field>
    </FieldGroup>
  );
}
