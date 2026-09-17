import { Checkbox } from "@/components/ui/checkbox";
import { Field, FieldLabel } from "@/components/ui/field";
import { Input } from "@/components/ui/input";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Textarea } from "@/components/ui/textarea";
import type { SessionFormField, SessionFormSchema } from "@/lib/types/formSchema";

interface DynamicFormProps {
  schema: SessionFormSchema;
  values: Record<string, unknown>;
  onChange: (values: Record<string, unknown>) => void;
}

function renderCampo(campo: SessionFormField, valor: unknown, onChangeValor: (v: unknown) => void) {
  switch (campo.type) {
    case "textarea":
      return (
        <Textarea
          id={campo.key}
          rows={3}
          value={(valor as string) ?? ""}
          onChange={(e) => onChangeValor(e.target.value)}
        />
      );
    case "select":
      return (
        <Select value={(valor as string) ?? ""} onValueChange={onChangeValor}>
          <SelectTrigger id={campo.key} className="w-full">
            <SelectValue placeholder="Selecione" />
          </SelectTrigger>
          <SelectContent>
            {campo.options?.map((opcao) => (
              <SelectItem key={opcao} value={opcao}>
                {opcao}
              </SelectItem>
            ))}
          </SelectContent>
        </Select>
      );
    case "number":
      return (
        <Input
          id={campo.key}
          type="number"
          value={(valor as string) ?? ""}
          onChange={(e) => onChangeValor(e.target.value)}
        />
      );
    case "date":
      return (
        <Input
          id={campo.key}
          type="date"
          value={(valor as string) ?? ""}
          onChange={(e) => onChangeValor(e.target.value)}
        />
      );
    case "boolean":
      return <Checkbox checked={(valor as boolean) ?? false} onCheckedChange={(v) => onChangeValor(v === true)} />;
    default:
      return (
        <Input id={campo.key} value={(valor as string) ?? ""} onChange={(e) => onChangeValor(e.target.value)} />
      );
  }
}

/**
 * Renderiza o formulario da sessao a partir do JSON schema (Anexos A.1-A.5)
 * correspondente ao TipoSessao do atendimento — ver src/templates/sessionSchemas.
 */
export function DynamicForm({ schema, values, onChange }: DynamicFormProps) {
  return (
    <div className="space-y-4">
      <p className="text-sm font-semibold text-foreground">{schema.title}</p>
      {schema.fields.map((campo) => (
        <Field key={campo.key}>
          <FieldLabel htmlFor={campo.key}>
            {campo.label}
            {campo.required && <span className="text-sus-red"> *</span>}
          </FieldLabel>
          {renderCampo(campo, values[campo.key], (v) => onChange({ ...values, [campo.key]: v }))}
        </Field>
      ))}
    </div>
  );
}
