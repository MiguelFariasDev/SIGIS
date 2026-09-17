export interface FamilyComposition {
  id: string;
  personId: string;
  fatherName?: string;
  fatherEducation?: string;
  fatherOccupation?: string;
  motherName?: string;
  motherEducation?: string;
  motherOccupation?: string;
  siblingsCount?: number;
  siblingsAges?: string;
  householdMembersCount?: number;
  parentsMaritalStatus?: string;
  filiationType?: "Natural" | "Adotivo";
  plannedPregnancy?: boolean;
  pregnanciesCount?: number;
  abortionsCount?: number;
  pregnancyHealthIssue?: string;
  deliveryType?: "Normal" | "Cesarea";
  medicationDuringPregnancy?: string;
  createdAt: string;
  updatedAt: string;
}

export type UpsertFamilyCompositionRequest = Omit<
  FamilyComposition,
  "id" | "personId" | "createdAt" | "updatedAt"
>;
