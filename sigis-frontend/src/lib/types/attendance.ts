import type { StatusComparecimento, TipoSessao } from "./enums";

/**
 * Registro de atendimento — mantem `dadosFormulario` em portugues
 * (nome de campo definido pelo dicionario oficial do backend).
 */
export interface Attendance {
  id: string;
  personId: string;
  personName: string;
  unitId: string;
  unitCode: string;
  professionalId: string;
  professionalName: string;
  dateTime: string; // ISO timestamp
  sessionType: TipoSessao;
  attendanceStatus: StatusComparecimento;
  triagedByProfessionalId?: string;
  triagedByProfessionalName?: string;
  mainComplaint?: string;
  internalReferrals?: string[];
  dadosFormulario: Record<string, unknown>;
  sessionNumber: number;
}

export interface CreateAttendanceRequest {
  personId: string;
  unitId: string;
  professionalId: string;
  sessionType: TipoSessao;
  dateTime: string;
  triagedByProfessionalId?: string;
  mainComplaint?: string;
  internalReferrals?: string[];
}

export interface UpdateAttendanceRequest {
  attendanceStatus: StatusComparecimento;
  dadosFormulario: Record<string, unknown>;
  triagedByProfessionalId?: string;
  mainComplaint?: string;
  internalReferrals?: string[];
}
