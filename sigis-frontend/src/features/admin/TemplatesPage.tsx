import { Copy, FileJson } from "lucide-react";
import { useState } from "react";
import { toast } from "sonner";
import { PageHeader } from "@/components/common/PageHeader";
import { Button } from "@/components/ui/button";
import { SESSION_SCHEMAS } from "@/templates/sessionSchemas";
import { TIPO_SESSAO_LABEL } from "@/lib/utils/constants";
import type { TipoSessao } from "@/lib/types/enums";

/** T19 — templates de sessao (Anexos A.1-A.5), visao admin/coordenador. */
export function TemplatesPage() {
  const tipos = Object.keys(SESSION_SCHEMAS) as TipoSessao[];
  const [selecionado, setSelecionado] = useState<TipoSessao>(tipos[0]);

  const schema = SESSION_SCHEMAS[selecionado];

  return (
    <div>
      <PageHeader
        title="Templates de sessao"
        description="Os 5 formularios dinamicos (Anexos A.1-A.5) usados no registro de atendimento (T08)."
      />

      <div className="grid grid-cols-1 gap-4 lg:grid-cols-[240px_1fr]">
        <div className="space-y-1.5">
          {tipos.map((tipo) => (
            <button
              key={tipo}
              type="button"
              onClick={() => setSelecionado(tipo)}
              className={`w-full rounded-lg border px-3 py-2 text-left text-sm font-medium transition-colors ${
                selecionado === tipo
                  ? "border-sus-blue bg-sus-blue-light text-sus-blue-dark"
                  : "border-border bg-white text-foreground hover:bg-neutral-50"
              }`}
            >
              {TIPO_SESSAO_LABEL[tipo]}
            </button>
          ))}
        </div>

        <div className="rounded-xl border border-border bg-white p-5">
          <div className="mb-3 flex items-center justify-between">
            <p className="flex items-center gap-2 text-sm font-semibold text-foreground">
              <FileJson className="size-4" />
              {schema.title}
            </p>
            <Button
              size="sm"
              variant="outline"
              onClick={() => toast.info("Duplicacao de template ainda nao disponivel neste prototipo.")}
            >
              <Copy className="size-3.5" />
              Duplicar template
            </Button>
          </div>

          <ul className="mb-4 divide-y divide-border rounded-lg border border-border">
            {schema.fields.map((campo) => (
              <li key={campo.key} className="flex items-center justify-between px-3 py-2 text-sm">
                <span className="text-foreground">
                  {campo.label}
                  {campo.required && <span className="text-sus-red"> *</span>}
                </span>
                <span className="text-xs text-muted-foreground">{campo.type}</span>
              </li>
            ))}
          </ul>

          <p className="mb-1.5 text-xs font-medium text-muted-foreground">JSON schema</p>
          <pre className="max-h-96 overflow-auto rounded-lg bg-neutral-950 p-4 text-xs text-neutral-100">
            {JSON.stringify(schema, null, 2)}
          </pre>
        </div>
      </div>
    </div>
  );
}
