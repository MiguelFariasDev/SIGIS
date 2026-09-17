import { pessoasMock } from "./data/pessoas.mock";
import { unidadesMock } from "./data/unidades.mock";
import { profissionaisMock } from "./data/profissionais.mock";
import { filasMock } from "./data/filas.mock";
import { atendimentosMock } from "./data/atendimentos.mock";
import { encaminhamentosMock } from "./data/encaminhamentos.mock";
import { duplicidadesMock } from "./data/duplicidades.mock";
import { auditoriaMock } from "./data/auditoria.mock";
import { legalBasisMock } from "./data/legalBasis.mock";
import { clinicalProfilesMock } from "./data/clinicalProfiles.mock";
import { schoolHistoriesMock } from "./data/schoolHistories.mock";
import { familyCompositionsMock } from "./data/familyCompositions.mock";
import { developmentMilestonesMock } from "./data/developmentMilestones.mock";
import { learningDifficultiesMock } from "./data/learningDifficulties.mock";
import { concurrentTreatmentsMock } from "./data/concurrentTreatments.mock";
import { consentsMock } from "./data/consents.mock";
import type { Person } from "@/lib/types/person";
import type { FilaAtendimento } from "@/lib/types/fila";
import type { Attendance } from "@/lib/types/attendance";
import type { Encaminhamento } from "@/lib/types/encaminhamento";
import type { AlertaDuplicidade } from "@/lib/types/duplicidade";
import type { LogAcesso } from "@/lib/types/logAcesso";
import type { LegalBasis } from "@/lib/types/legalBasis";
import type { PersonClinicalProfile } from "@/lib/types/personClinicalProfile";
import type { SchoolHistory } from "@/lib/types/schoolHistory";
import type { FamilyComposition } from "@/lib/types/familyComposition";
import type { DevelopmentMilestones } from "@/lib/types/developmentMilestones";
import type { LearningDifficulties } from "@/lib/types/learningDifficulties";
import type { ConcurrentTreatment } from "@/lib/types/concurrentTreatment";
import type { PersonConsent } from "@/lib/types/personConsent";
import type { Guardian } from "@/lib/types/guardian";

/**
 * Estado em memoria do "backend fake". Reseta a cada reload de pagina.
 * Quando o backend real existir, basta desligar VITE_USE_MOCKS.
 */
export const db = {
  pessoas: [...pessoasMock] as Person[],
  unidades: unidadesMock,
  profissionais: profissionaisMock,
  filas: [...filasMock] as FilaAtendimento[],
  atendimentos: [...atendimentosMock] as Attendance[],
  encaminhamentos: [...encaminhamentosMock] as Encaminhamento[],
  duplicidades: [...duplicidadesMock] as AlertaDuplicidade[],
  auditoria: [...auditoriaMock] as LogAcesso[],
  legalBasis: legalBasisMock as LegalBasis[],
  clinicalProfiles: [...clinicalProfilesMock] as PersonClinicalProfile[],
  schoolHistories: [...schoolHistoriesMock] as SchoolHistory[],
  familyCompositions: [...familyCompositionsMock] as FamilyComposition[],
  developmentMilestones: [...developmentMilestonesMock] as DevelopmentMilestones[],
  learningDifficulties: [...learningDifficultiesMock] as LearningDifficulties[],
  concurrentTreatments: [...concurrentTreatmentsMock] as ConcurrentTreatment[],
  consents: [...consentsMock] as PersonConsent[],
  guardians: [] as Guardian[],
};

let contador = 100;
export function gerarId(prefixo: string): string {
  contador += 1;
  return `${prefixo}-${contador}`;
}

function normalizar(texto: string): string {
  return texto
    .normalize("NFD")
    .replace(/[̀-ͯ]/g, "")
    .toLowerCase()
    .trim();
}

function bigramas(texto: string): Set<string> {
  const normalizado = normalizar(texto).replace(/\s+/g, " ");
  const conjunto = new Set<string>();
  for (let i = 0; i < normalizado.length - 1; i++) {
    conjunto.add(normalizado.slice(i, i + 2));
  }
  return conjunto;
}

/** Aproximacao do coeficiente de Dice, no mesmo espirito do similarity() do pg_trgm. */
export function calcularSimilaridadeNome(a: string, b: string): number {
  const setA = bigramas(a);
  const setB = bigramas(b);
  if (setA.size === 0 || setB.size === 0) return 0;
  let intersecao = 0;
  for (const bigrama of setA) {
    if (setB.has(bigrama)) intersecao += 1;
  }
  return (2 * intersecao) / (setA.size + setB.size);
}

export interface CandidatoDuplicidade {
  pessoa: Person;
  score: number;
}

const LIMIAR_SIMILARIDADE = 0.75;

export function buscarCandidatosDuplicidade(
  nomeCompleto: string,
  dataNascimento: string,
  ignorarId?: string,
): CandidatoDuplicidade[] {
  return db.pessoas
    .filter((p) => p.id !== ignorarId && p.birthDate === dataNascimento)
    .map((p) => ({ pessoa: p, score: calcularSimilaridadeNome(nomeCompleto, p.fullName) }))
    .filter((c) => c.score >= LIMIAR_SIMILARIDADE)
    .sort((a, b) => b.score - a.score);
}
