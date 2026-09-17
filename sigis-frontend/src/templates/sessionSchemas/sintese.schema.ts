import type { SessionFormSchema } from "@/lib/types/formSchema";

/** Anexo A.5 — Sintese de acompanhamento (NAPE). */
export const sinteseSchema: SessionFormSchema = {
  sessionType: "SINTESE",
  title: "Sintese de acompanhamento",
  fields: [
    { key: "periodoAcompanhamento", label: "Periodo de acompanhamento", type: "text", required: true },
    { key: "evolucaoGeral", label: "Evolucao geral", type: "textarea", required: true },
    { key: "profissionaisEnvolvidos", label: "Profissionais envolvidos", type: "textarea" },
    { key: "recomendacoesFuturas", label: "Recomendacoes futuras", type: "textarea" },
  ],
};
