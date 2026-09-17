import { Controller, type Control, type FieldErrors, type UseFormRegister } from "react-hook-form";
import { Field, FieldError, FieldGroup, FieldLabel, FieldLegend, FieldSet } from "@/components/ui/field";
import { Input } from "@/components/ui/input";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { mascararCns, mascararCpf, mascararTelefone } from "@/lib/utils/formatters";
import type { CadastroPessoaFormValues } from "../schemas";

interface DadosBasicosFormProps {
  register: UseFormRegister<CadastroPessoaFormValues>;
  control: Control<CadastroPessoaFormValues>;
  errors: FieldErrors<CadastroPessoaFormValues>;
}

export function DadosBasicosForm({ register, control, errors }: DadosBasicosFormProps) {
  return (
    <div className="space-y-8">
      <FieldSet>
        <FieldLegend>Dados da pessoa</FieldLegend>
        <FieldGroup>
          <Field data-invalid={!!errors.fullName}>
            <FieldLabel htmlFor="fullName">Nome completo</FieldLabel>
            <Input id="fullName" {...register("fullName")} />
            <FieldError errors={errors.fullName ? [errors.fullName] : undefined} />
          </Field>

          <Field orientation="responsive" data-invalid={!!errors.birthDate}>
            <FieldLabel htmlFor="birthDate">Data de nascimento</FieldLabel>
            <Input id="birthDate" type="date" {...register("birthDate")} />
            <FieldError errors={errors.birthDate ? [errors.birthDate] : undefined} />
          </Field>

          <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
            <Field data-invalid={!!errors.cns}>
              <FieldLabel htmlFor="cns">CNS (Cartao Nacional de Saude)</FieldLabel>
              <Controller
                control={control}
                name="cns"
                render={({ field }) => (
                  <Input
                    id="cns"
                    inputMode="numeric"
                    placeholder="000 0000 0000 0000"
                    value={field.value ?? ""}
                    onChange={(e) => field.onChange(mascararCns(e.target.value))}
                  />
                )}
              />
              <FieldError errors={errors.cns ? [errors.cns] : undefined} />
            </Field>

            <Field data-invalid={!!errors.cpf}>
              <FieldLabel htmlFor="cpf">CPF (opcional)</FieldLabel>
              <Controller
                control={control}
                name="cpf"
                render={({ field }) => (
                  <Input
                    id="cpf"
                    inputMode="numeric"
                    placeholder="000.000.000-00"
                    value={field.value ?? ""}
                    onChange={(e) => field.onChange(mascararCpf(e.target.value))}
                  />
                )}
              />
              <FieldError errors={errors.cpf ? [errors.cpf] : undefined} />
            </Field>
          </div>

          <Field>
            <FieldLabel htmlFor="motherName">Nome da mae</FieldLabel>
            <Input id="motherName" {...register("motherName")} />
          </Field>

          <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
            <Field>
              <FieldLabel htmlFor="gender">Sexo</FieldLabel>
              <Controller
                control={control}
                name="gender"
                render={({ field }) => (
                  <Select value={field.value ?? ""} onValueChange={field.onChange}>
                    <SelectTrigger id="gender" className="w-full">
                      <SelectValue placeholder="Selecione" />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value="Feminino">Feminino</SelectItem>
                      <SelectItem value="Masculino">Masculino</SelectItem>
                    </SelectContent>
                  </Select>
                )}
              />
            </Field>

            <Field>
              <FieldLabel htmlFor="raceColor">Cor/raca</FieldLabel>
              <Controller
                control={control}
                name="raceColor"
                render={({ field }) => (
                  <Select value={field.value ?? ""} onValueChange={field.onChange}>
                    <SelectTrigger id="raceColor" className="w-full">
                      <SelectValue placeholder="Selecione" />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value="Branca">Branca</SelectItem>
                      <SelectItem value="Preta">Preta</SelectItem>
                      <SelectItem value="Parda">Parda</SelectItem>
                      <SelectItem value="Amarela">Amarela</SelectItem>
                      <SelectItem value="Indigena">Indigena</SelectItem>
                    </SelectContent>
                  </Select>
                )}
              />
            </Field>
          </div>
        </FieldGroup>
      </FieldSet>

      <FieldSet>
        <FieldLegend>Contato</FieldLegend>
        <FieldGroup>
          <Field>
            <FieldLabel htmlFor="phone">Telefone</FieldLabel>
            <Controller
              control={control}
              name="phone"
              render={({ field }) => (
                <Input
                  id="phone"
                  inputMode="numeric"
                  placeholder="(88) 90000-0000"
                  value={field.value ?? ""}
                  onChange={(e) => field.onChange(mascararTelefone(e.target.value))}
                />
              )}
            />
          </Field>

          <Field data-invalid={!!errors.email}>
            <FieldLabel htmlFor="email">E-mail</FieldLabel>
            <Input id="email" type="email" {...register("email")} />
            <FieldError errors={errors.email ? [errors.email] : undefined} />
          </Field>

          <div className="grid grid-cols-1 gap-4 sm:grid-cols-3">
            <Field className="sm:col-span-2">
              <FieldLabel htmlFor="street">Rua</FieldLabel>
              <Input id="street" {...register("street")} />
            </Field>
            <Field>
              <FieldLabel htmlFor="number">Numero</FieldLabel>
              <Input id="number" {...register("number")} />
            </Field>
          </div>

          <div className="grid grid-cols-1 gap-4 sm:grid-cols-3">
            <Field>
              <FieldLabel htmlFor="neighborhood">Bairro</FieldLabel>
              <Input id="neighborhood" {...register("neighborhood")} />
            </Field>
            <Field>
              <FieldLabel htmlFor="city">Municipio</FieldLabel>
              <Input id="city" placeholder="Crateus" {...register("city")} />
            </Field>
            <Field>
              <FieldLabel htmlFor="state">UF</FieldLabel>
              <Input id="state" placeholder="CE" maxLength={2} {...register("state")} />
            </Field>
          </div>
        </FieldGroup>
      </FieldSet>
    </div>
  );
}
