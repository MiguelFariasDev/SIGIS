import type { LearningDifficulties } from "@/lib/types/learningDifficulties";

export const learningDifficultiesMock: LearningDifficulties[] = [
  {
    id: "dificuldade-01",
    personId: "pessoa-01",
    type: "Atencao",
    severity: "Moderada",
    assessmentDate: "2026-01-10",
    notes: "Dificuldade de atencao sustentada em atividades de sala.",
    createdAt: "2026-01-10T09:00:00Z",
  },
  {
    id: "dificuldade-02",
    personId: "pessoa-01",
    type: "Escrita",
    severity: "Leve",
    assessmentDate: "2026-01-10",
    createdAt: "2026-01-10T09:00:00Z",
  },
];
