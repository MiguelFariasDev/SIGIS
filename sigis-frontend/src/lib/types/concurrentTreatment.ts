export type DayOfWeek = "Monday" | "Tuesday" | "Wednesday" | "Thursday" | "Friday" | "Saturday" | "Sunday";

export interface ConcurrentTreatment {
  id: string;
  personId: string;
  specialty: string;
  location: string;
  professionalName: string;
  dayOfWeek: DayOfWeek;
  startTime: string;
  endTime: string;
  notes?: string;
  createdAt: string;
  updatedAt: string;
}

export type CreateConcurrentTreatmentRequest = Omit<
  ConcurrentTreatment,
  "id" | "personId" | "createdAt" | "updatedAt"
>;
