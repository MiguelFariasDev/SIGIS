export interface PersonClinicalProfile {
  id: string;
  personId: string;
  medicalRecordNumber?: string;
  clinicalHypothesis?: string;
  apsReferenceUnitId?: string;
  createdAt: string;
  updatedAt: string;
}

export type UpsertClinicalProfileRequest = Omit<
  PersonClinicalProfile,
  "id" | "personId" | "createdAt" | "updatedAt"
>;
