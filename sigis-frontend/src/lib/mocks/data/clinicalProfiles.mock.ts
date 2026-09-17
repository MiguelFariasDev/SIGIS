import type { PersonClinicalProfile } from "@/lib/types/personClinicalProfile";

export const clinicalProfilesMock: PersonClinicalProfile[] = [
  {
    id: "perfil-clinico-01",
    personId: "pessoa-01",
    medicalRecordNumber: "NASF-2026-0032",
    clinicalHypothesis: "TEA nivel de suporte 1",
    apsReferenceUnitId: "unidade-nasf",
    createdAt: "2025-11-04T09:12:00Z",
    updatedAt: "2026-02-07T09:05:00Z",
  },
  {
    id: "perfil-clinico-02",
    personId: "pessoa-10",
    medicalRecordNumber: "NASF-2025-0118",
    clinicalHypothesis: "TEA confirmado - nivel de suporte 2",
    apsReferenceUnitId: "unidade-nasf",
    createdAt: "2025-06-11T10:00:00Z",
    updatedAt: "2026-02-12T14:30:00Z",
  },
];
