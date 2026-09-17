import type { TipoSessao } from "@/lib/types/enums";
import type { SessionFormSchema } from "@/lib/types/formSchema";
import { anamnesePsiSchema } from "./anamnesePsi.schema";
import { anamnesePsicopedSchema } from "./anamnesePsicoped.schema";
import { instrumentalEdFisicaSchema } from "./instrumentalEdFisica.schema";
import { prontuarioNasfSchema } from "./prontuarioNasf.schema";
import { sinteseSchema } from "./sintese.schema";

/** Os 5 templates de sessao (Anexos A.1-A.5), indexados por tipo. */
export const SESSION_SCHEMAS: Record<TipoSessao, SessionFormSchema> = {
  PRONTUARIO_NASF: prontuarioNasfSchema,
  ANAMNESE_PSI: anamnesePsiSchema,
  ANAMNESE_PSICOPED: anamnesePsicopedSchema,
  INSTRUMENTAL_EDFISICA: instrumentalEdFisicaSchema,
  SINTESE: sinteseSchema,
};

export {
  anamnesePsiSchema,
  anamnesePsicopedSchema,
  instrumentalEdFisicaSchema,
  prontuarioNasfSchema,
  sinteseSchema,
};
