import type { SessionFormSchema } from "@/lib/types/formSchema";

/** Anexo A.2 — Anamnese psicologica (NAPE). */
export const anamnesePsiSchema: SessionFormSchema = {
  sessionType: "ANAMNESE_PSI",
  title: "Anamnese psicologica",
  fields: [
    { key: "queixaPrincipal", label: "Queixa principal", type: "textarea", required: true },
    { key: "historicoDesenvolvimento", label: "Historico do desenvolvimento", type: "textarea" },
    { key: "historicoFamiliar", label: "Historico familiar", type: "textarea" },
    { key: "comportamentoObservado", label: "Comportamento observado na sessao", type: "textarea" },
    { key: "hipoteseDiagnostica", label: "Hipotese diagnostica", type: "textarea" },
    { key: "encaminhamentosSugeridos", label: "Encaminhamentos sugeridos", type: "textarea" },
  ],
};
