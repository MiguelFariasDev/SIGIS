import type { SessionFormSchema } from "@/lib/types/formSchema";

/** Anexo A.4 — Instrumental de avaliacao de educacao fisica (NAPE). */
export const instrumentalEdFisicaSchema: SessionFormSchema = {
  sessionType: "INSTRUMENTAL_EDFISICA",
  title: "Instrumental de educacao fisica",
  fields: [
    { key: "avaliacaoMotora", label: "Avaliacao motora", type: "textarea", required: true },
    {
      key: "coordenacaoMotora",
      label: "Coordenacao motora",
      type: "select",
      options: ["Adequada", "Em desenvolvimento", "Comprometida"],
    },
    {
      key: "equilibrio",
      label: "Equilibrio",
      type: "select",
      options: ["Adequado", "Em desenvolvimento", "Comprometido"],
    },
    { key: "atividadesRecomendadas", label: "Atividades recomendadas", type: "textarea" },
  ],
};
