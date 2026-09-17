export type LearningDifficultyType =
  | "Leitura"
  | "Escrita"
  | "Calculo"
  | "Atencao"
  | "Concentracao"
  | "CoordenacaoMotora"
  | "Outro";

export type LearningDifficultySeverity = "Leve" | "Moderada" | "Severa";

export interface LearningDifficulties {
  id: string;
  personId: string;
  type: LearningDifficultyType;
  severity?: LearningDifficultySeverity;
  assessmentDate?: string;
  notes?: string;
  createdAt: string;
}

export interface CreateLearningDifficultyRequest {
  type: LearningDifficultyType;
  severity?: LearningDifficultySeverity;
  assessmentDate?: string;
  notes?: string;
}
