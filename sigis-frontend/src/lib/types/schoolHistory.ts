export type SchoolStatus = "Ativo" | "Transferido" | "Concluido" | "Evadido";

export interface SchoolHistory {
  id: string;
  personId: string;
  schoolName: string;
  grade: string;
  shift?: string;
  classGroup?: string;
  schoolYear: number;
  startDate?: string;
  endDate?: string;
  status: SchoolStatus;
  notes?: string;
  createdAt: string;
  createdByProfessionalId: string;
}

export interface CreateSchoolHistoryRequest {
  schoolName: string;
  grade: string;
  shift?: string;
  classGroup?: string;
  schoolYear: number;
  startDate?: string;
  endDate?: string;
  status: SchoolStatus;
  notes?: string;
}
