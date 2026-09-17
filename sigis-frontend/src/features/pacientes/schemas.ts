import { z } from "zod";
import { validarCns, validarCpf } from "@/lib/utils/validators";

export const cadastroPessoaSchema = z
  .object({
    // Secao 1 — Dados pessoais
    fullName: z.string().min(3, "Informe o nome completo."),
    birthDate: z.string().min(1, "Informe a data de nascimento."),
    cns: z
      .string()
      .optional()
      .refine((v) => !v || v.replace(/\D/g, "").length === 0 || validarCns(v), "CNS invalido."),
    cpf: z
      .string()
      .optional()
      .refine((v) => !v || v.replace(/\D/g, "").length === 0 || validarCpf(v), "CPF invalido."),
    motherName: z.string().optional(),
    gender: z.string().optional(),
    raceColor: z.string().optional(),
    phone: z.string().optional(),
    email: z.string().email("E-mail invalido.").optional().or(z.literal("")),
    street: z.string().optional(),
    number: z.string().optional(),
    neighborhood: z.string().optional(),
    city: z.string().optional(),
    state: z.string().optional(),

    // Secao 2 — Dados escolares
    naturality: z.string().optional(),
    currentSchool: z.string().optional(),
    grade: z.string().optional(),
    shift: z.string().optional(),
    classGroup: z.string().optional(),
    zone: z.enum(["Urbana", "Rural"]).optional(),
    schoolEnrollment: z.string().optional(),
    referredBySchool: z.boolean(),
    needsSpecialEducation: z.boolean(),
    attendsTutoring: z.boolean(),
    hasFailedGrade: z.boolean(),
    disabilityTypes: z.array(z.string()),

    // Secao 3 — Perfil clinico NASF
    medicalRecordNumber: z.string().optional(),
    clinicalHypothesis: z.string().optional(),
    apsReferenceUnitId: z.string().optional(),

    // Secao 4 — Composicao familiar
    fatherName: z.string().optional(),
    fatherEducation: z.string().optional(),
    fatherOccupation: z.string().optional(),
    familyMotherName: z.string().optional(),
    motherEducation: z.string().optional(),
    motherOccupation: z.string().optional(),
    siblingsCount: z.string().optional(),
    siblingsAges: z.string().optional(),
    householdMembersCount: z.string().optional(),
    parentsMaritalStatus: z.string().optional(),
    filiationType: z.enum(["Natural", "Adotivo"]).optional(),
    plannedPregnancy: z.boolean(),
    pregnanciesCount: z.string().optional(),
    abortionsCount: z.string().optional(),
    pregnancyHealthIssue: z.string().optional(),
    deliveryType: z.enum(["Normal", "Cesarea"]).optional(),
    medicationDuringPregnancy: z.string().optional(),

    // Secao 5 — Desenvolvimento
    ageWalkedMonths: z.string().optional(),
    ageTalkedMonths: z.string().optional(),
    locomotionDifficulty: z.boolean(),
    coordinationDifficulty: z.boolean(),
    visualDifficulty: z.boolean(),
    hearingDifficulty: z.boolean(),
    speechProblems: z.string().optional(),
    commandComprehension: z.string().optional(),
    manualDominance: z.enum(["Destro", "Canhoto", "Ambidestro"]).optional(),

    // Secao 6 — Consentimentos LGPD
    consentClinical: z.boolean().refine((v) => v === true, {
      message: "O consentimento para atendimento clinico e obrigatorio.",
    }),
    consentEducational: z.boolean(),
    consentSocialAssistance: z.boolean(),
    consentResearch: z.boolean(),
    consentCollectedAt: z.string().optional(),
    consentCollectedBy: z.string().optional(),

    // Secao 7 — Responsavel
    guardianName: z.string().optional(),
    guardianCns: z.string().optional(),
    guardianBirthDate: z.string().optional(),
    guardianRelationship: z.string().optional(),
  })
  .refine((data) => !!data.cns || !!data.cpf, {
    message: "Informe ao menos o CNS ou o CPF da pessoa.",
    path: ["cns"],
  });

export type CadastroPessoaFormValues = z.infer<typeof cadastroPessoaSchema>;

/** Converte um campo numerico textual do formulario para number|undefined. */
export function paraNumero(valor: string | undefined): number | undefined {
  if (!valor || valor.trim() === "") return undefined;
  const n = Number(valor);
  return Number.isNaN(n) ? undefined : n;
}
