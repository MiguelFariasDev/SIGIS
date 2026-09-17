import { zodResolver } from "@hookform/resolvers/zod";
import { useQuery } from "@tanstack/react-query";
import { Check, ChevronsUpDown, Send } from "lucide-react";
import { useState } from "react";
import { Controller, useForm } from "react-hook-form";
import { z } from "zod";
import { Button } from "@/components/ui/button";
import { Command, CommandEmpty, CommandGroup, CommandInput, CommandItem, CommandList } from "@/components/ui/command";
import { Field, FieldError, FieldGroup, FieldLabel } from "@/components/ui/field";
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Textarea } from "@/components/ui/textarea";
import { buscarPessoas } from "@/features/pacientes/api";
import { unidadesMock } from "@/lib/mocks/data/unidades.mock";
import { PrioridadeFila } from "@/lib/types/enums";
import type { CriarEncaminhamentoRequest } from "@/lib/types/encaminhamento";
import { PRIORIDADE_LABEL } from "@/lib/utils/constants";
import { cn } from "@/lib/utils";
import { useAuthStore } from "@/stores/authStore";

const schema = z.object({
  pessoaId: z.string().min(1, "Selecione o paciente."),
  pessoaNome: z.string(),
  unidadeDestinoId: z.string().min(1, "Selecione a unidade de destino."),
  motivo: z.string().min(5, "Descreva o motivo do encaminhamento."),
  prioridade: z.nativeEnum(PrioridadeFila),
});

type FormValues = z.infer<typeof schema>;

interface EncaminhamentoFormProps {
  pessoaIdInicial?: string;
  pessoaNomeInicial?: string;
  onSubmit: (request: CriarEncaminhamentoRequest) => void;
  isSubmitting?: boolean;
}

export function EncaminhamentoForm({
  pessoaIdInicial,
  pessoaNomeInicial,
  onSubmit,
  isSubmitting,
}: EncaminhamentoFormProps) {
  const usuario = useAuthStore((s) => s.usuario);
  const [termoBusca, setTermoBusca] = useState("");
  const [popoverAberto, setPopoverAberto] = useState(false);

  const buscaQuery = useQuery({
    queryKey: ["pessoas", "busca-encaminhamento", termoBusca],
    queryFn: () => buscarPessoas({ term: termoBusca }),
    enabled: termoBusca.length >= 2,
  });

  const {
    control,
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<FormValues>({
    resolver: zodResolver(schema),
    defaultValues: {
      pessoaId: pessoaIdInicial ?? "",
      pessoaNome: pessoaNomeInicial ?? "",
      unidadeDestinoId: "",
      motivo: "",
      prioridade: PrioridadeFila.CURTO_PRAZO,
    },
  });

  function handleFormSubmit(values: FormValues) {
    if (!usuario) return;
    onSubmit({
      personId: values.pessoaId,
      unidadeOrigemId: usuario.unidadeId,
      unidadeDestinoId: values.unidadeDestinoId,
      motivo: values.motivo,
      prioridade: values.prioridade,
    });
  }

  return (
    <form onSubmit={handleSubmit(handleFormSubmit)} noValidate>
      <FieldGroup>
        <Field data-invalid={!!errors.pessoaId}>
          <FieldLabel>Paciente</FieldLabel>
          <Controller
            control={control}
            name="pessoaId"
            render={({ field }) => (
              <Popover open={popoverAberto} onOpenChange={setPopoverAberto}>
                <PopoverTrigger asChild>
                  <Button
                    type="button"
                    variant="outline"
                    role="combobox"
                    className="w-full justify-between font-normal"
                    disabled={!!pessoaIdInicial}
                  >
                    {field.value ? pessoaNomeInicial || termoBusca || "Paciente selecionado" : "Buscar paciente..."}
                    <ChevronsUpDown className="size-4 opacity-50" />
                  </Button>
                </PopoverTrigger>
                <PopoverContent className="w-(--radix-popover-trigger-width) p-0" align="start">
                  <Command shouldFilter={false}>
                    <CommandInput
                      placeholder="Digite o nome, CNS ou CPF..."
                      value={termoBusca}
                      onValueChange={setTermoBusca}
                    />
                    <CommandList>
                      <CommandEmpty>
                        {termoBusca.length < 2 ? "Digite ao menos 2 caracteres." : "Nenhum paciente encontrado."}
                      </CommandEmpty>
                      <CommandGroup>
                        {buscaQuery.data?.map((pessoa) => (
                          <CommandItem
                            key={pessoa.id}
                            value={pessoa.id}
                            onSelect={() => {
                              field.onChange(pessoa.id);
                              setTermoBusca(pessoa.fullName);
                              setPopoverAberto(false);
                            }}
                          >
                            <Check className={cn("size-4", field.value === pessoa.id ? "opacity-100" : "opacity-0")} />
                            {pessoa.fullName}
                          </CommandItem>
                        ))}
                      </CommandGroup>
                    </CommandList>
                  </Command>
                </PopoverContent>
              </Popover>
            )}
          />
          <FieldError errors={errors.pessoaId ? [errors.pessoaId] : undefined} />
        </Field>

        <Field data-invalid={!!errors.unidadeDestinoId}>
          <FieldLabel htmlFor="unidadeDestinoId">Unidade de destino</FieldLabel>
          <Controller
            control={control}
            name="unidadeDestinoId"
            render={({ field }) => (
              <Select value={field.value} onValueChange={field.onChange}>
                <SelectTrigger id="unidadeDestinoId" className="w-full">
                  <SelectValue placeholder="Selecione a unidade" />
                </SelectTrigger>
                <SelectContent>
                  {unidadesMock
                    .filter((u) => u.id !== usuario?.unidadeId)
                    .map((unidade) => (
                      <SelectItem key={unidade.id} value={unidade.id}>
                        {unidade.nome} ({unidade.sigla})
                      </SelectItem>
                    ))}
                </SelectContent>
              </Select>
            )}
          />
          <FieldError errors={errors.unidadeDestinoId ? [errors.unidadeDestinoId] : undefined} />
        </Field>

        <Field data-invalid={!!errors.prioridade}>
          <FieldLabel htmlFor="prioridade">Prioridade</FieldLabel>
          <Controller
            control={control}
            name="prioridade"
            render={({ field }) => (
              <Select value={field.value} onValueChange={field.onChange}>
                <SelectTrigger id="prioridade" className="w-full">
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  {Object.values(PrioridadeFila).map((prioridade) => (
                    <SelectItem key={prioridade} value={prioridade}>
                      {PRIORIDADE_LABEL[prioridade]}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            )}
          />
        </Field>

        <Field data-invalid={!!errors.motivo}>
          <FieldLabel htmlFor="motivo">Motivo do encaminhamento</FieldLabel>
          <Textarea id="motivo" rows={4} {...register("motivo")} />
          <FieldError errors={errors.motivo ? [errors.motivo] : undefined} />
        </Field>

        <div className="flex justify-end">
          <Button type="submit" className="bg-sus-blue hover:bg-sus-blue-dark" disabled={isSubmitting}>
            <Send className="size-4" />
            {isSubmitting ? "Enviando..." : "Encaminhar"}
          </Button>
        </div>
      </FieldGroup>
    </form>
  );
}
