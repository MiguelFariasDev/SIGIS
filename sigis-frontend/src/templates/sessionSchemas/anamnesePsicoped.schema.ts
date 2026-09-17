import type { SessionFormSchema } from "@/lib/types/formSchema";

/** Anexo A.3 — Anamnese psicopedagogica (NAPE). */
export const anamnesePsicopedSchema: SessionFormSchema = {
  sessionType: "ANAMNESE_PSICOPED",
  title: "Anamnese psicopedagogica",
  fields: [
    { key: "queixaEscolar", label: "Queixa escolar", type: "textarea", required: true },
    { key: "historicoEscolar", label: "Historico escolar", type: "textarea" },
    { key: "habilidadesCognitivas", label: "Habilidades cognitivas observadas", type: "textarea" },
    { key: "dificuldadesAprendizagem", label: "Dificuldades de aprendizagem", type: "textarea" },
    { key: "planoIntervencao", label: "Plano de intervencao", type: "textarea" },
  ],
};
