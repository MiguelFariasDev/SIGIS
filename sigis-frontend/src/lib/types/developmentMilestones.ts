export type ManualDominance = "Destro" | "Canhoto" | "Ambidestro";

export interface DevelopmentMilestones {
  id: string;
  personId: string;
  ageWalkedMonths?: number;
  ageTalkedMonths?: number;
  locomotionDifficulty?: boolean;
  coordinationDifficulty?: boolean;
  visualDifficulty?: boolean;
  hearingDifficulty?: boolean;
  speechProblems?: string;
  commandComprehension?: string;
  communicationForm?: string;
  manualDominance?: ManualDominance;
  createdAt: string;
  updatedAt: string;
}

export type UpsertDevelopmentMilestonesRequest = Omit<
  DevelopmentMilestones,
  "id" | "personId" | "createdAt" | "updatedAt"
>;
