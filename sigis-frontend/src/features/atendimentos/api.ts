import { apiClient } from "@/lib/api/client";
import { ENDPOINTS } from "@/lib/api/endpoints";
import type { Attendance, CreateAttendanceRequest, UpdateAttendanceRequest } from "@/lib/types/attendance";

export async function fetchAtendimento(id: string): Promise<Attendance> {
  const { data } = await apiClient.get<Attendance>(ENDPOINTS.atendimentos.porId(id));
  return data;
}

export async function fetchAtendimentosPorPessoa(pessoaId: string): Promise<Attendance[]> {
  const { data } = await apiClient.get<Attendance[]>(ENDPOINTS.atendimentos.porPessoa(pessoaId));
  return data;
}

export async function criarAtendimento(request: CreateAttendanceRequest): Promise<Attendance> {
  const { data } = await apiClient.post<Attendance>(ENDPOINTS.atendimentos.base, request);
  return data;
}

export async function atualizarAtendimento(id: string, request: UpdateAttendanceRequest): Promise<Attendance> {
  const { data } = await apiClient.patch<Attendance>(ENDPOINTS.atendimentos.porId(id), request);
  return data;
}
