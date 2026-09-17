import { apiClient } from "@/lib/api/client";
import { ENDPOINTS } from "@/lib/api/endpoints";
import { mapAttendanceStatusFromApi, mapSessionTypeFromApi, mapSessionTypeToApi } from "@/lib/api/backendMappers";
import type { Attendance, CreateAttendanceRequest, UpdateAttendanceRequest } from "@/lib/types/attendance";

interface BackendAttendanceResponse {
  id: string;
  personId: string;
  personName: string;
  unitId: string;
  unitAcronym: string;
  professionalId: string;
  professionalName: string;
  dateTime: string;
  sessionType: string;
  sessionNumber: number;
  status: string;
  formData: string | null;
  triagedByProfessionalId?: string;
  mainComplaint?: string;
  createdAt: string;
}

function mapAttendanceFromApi(a: BackendAttendanceResponse): Attendance {
  let dadosFormulario: Record<string, unknown> = {};
  try {
    dadosFormulario = a.formData ? JSON.parse(a.formData) : {};
  } catch {
    dadosFormulario = {};
  }

  return {
    id: a.id,
    personId: a.personId,
    personName: a.personName,
    unitId: a.unitId,
    unitCode: a.unitAcronym,
    professionalId: a.professionalId,
    professionalName: a.professionalName,
    dateTime: a.dateTime,
    sessionType: mapSessionTypeFromApi(a.sessionType),
    attendanceStatus: mapAttendanceStatusFromApi(a.status),
    triagedByProfessionalId: a.triagedByProfessionalId,
    mainComplaint: a.mainComplaint,
    dadosFormulario,
    sessionNumber: a.sessionNumber,
  };
}

export async function fetchAtendimento(id: string): Promise<Attendance> {
  const { data } = await apiClient.get<BackendAttendanceResponse>(ENDPOINTS.atendimentos.porId(id));
  return mapAttendanceFromApi(data);
}

export async function fetchAtendimentosPorPessoa(pessoaId: string): Promise<Attendance[]> {
  const { data } = await apiClient.get<BackendAttendanceResponse[]>(ENDPOINTS.atendimentos.porPessoa(pessoaId));
  return data.map(mapAttendanceFromApi);
}

/** Único caso com suporte real (`POST /api/atendimentos`). */
export async function criarAtendimento(request: CreateAttendanceRequest): Promise<Attendance> {
  const { data } = await apiClient.post<BackendAttendanceResponse>(ENDPOINTS.atendimentos.base, {
    personId: request.personId,
    unitId: request.unitId,
    professionalId: request.professionalId,
    dateTime: request.dateTime,
    sessionType: mapSessionTypeToApi(request.sessionType),
    triagedByProfessionalId: request.triagedByProfessionalId,
    mainComplaint: request.mainComplaint,
    formData: undefined,
  });
  return mapAttendanceFromApi(data);
}

/**
 * O backend real só aceita `{comparecimento, mainComplaint?}` via
 * `PATCH /api/atendimentos/{id}/comparecimento` — `dadosFormulario` e
 * `internalReferrals` do `UpdateAttendanceRequest` não são persistidos
 * (gap documentado).
 */
export async function atualizarAtendimento(id: string, request: UpdateAttendanceRequest): Promise<Attendance> {
  const comparecimento = request.attendanceStatus === "COMPARECEU" ? "COMPARECEU" : "FALTOU";
  const { data } = await apiClient.patch<BackendAttendanceResponse>(ENDPOINTS.atendimentos.comparecimento(id), {
    comparecimento,
    mainComplaint: request.mainComplaint,
  });
  return mapAttendanceFromApi(data);
}
