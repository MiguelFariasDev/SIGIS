import { atendimentosHandlers } from "./atendimentos";
import { auditoriaHandlers } from "./auditoria";
import { authHandlers } from "./auth";
import { catalogosHandlers } from "./catalogos";
import { duplicidadesHandlers } from "./duplicidades";
import { encaminhamentosHandlers } from "./encaminhamentos";
import { filasHandlers } from "./filas";
import { indicadoresHandlers } from "./indicadores";
import { legalBasisHandlers } from "./legalBasis";
import { personProfileHandlers } from "./personProfile";
import { pessoasHandlers } from "./pessoas";

export const handlers = [
  ...authHandlers,
  ...catalogosHandlers,
  ...pessoasHandlers,
  ...personProfileHandlers,
  ...legalBasisHandlers,
  ...filasHandlers,
  ...atendimentosHandlers,
  ...encaminhamentosHandlers,
  ...duplicidadesHandlers,
  ...indicadoresHandlers,
  ...auditoriaHandlers,
];
