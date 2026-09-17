import type { SessionFormSchema } from "@/lib/types/formSchema";

/** Anexo A.1 — Prontuario clinico simples do NASF. */
export const prontuarioNasfSchema: SessionFormSchema = {
  sessionType: "PRONTUARIO_NASF",
  title: "Prontuario NASF",
  fields: [
    { key: "numeroProntuario", label: "Numero do prontuario", type: "text", required: true },
    { key: "hipoteseDiagnostica", label: "Hipotese diagnostica", type: "textarea", required: true },
    { key: "cns", label: "CNS do paciente", type: "text" },
    { key: "filiacao", label: "Filiacao", type: "text" },
    { key: "dadosResponsavel", label: "Dados do responsavel", type: "textarea" },
    { key: "condutaAdotada", label: "Conduta adotada", type: "textarea" },
  ],
};
