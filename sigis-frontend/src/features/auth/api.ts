import { apiClient } from "@/lib/api/client";
import { ENDPOINTS } from "@/lib/api/endpoints";
import { mapRbacRoleFromApi } from "@/lib/api/backendMappers";
import type { LoginRequest, LoginResponse } from "@/lib/types/auth";

/** Formato real da resposta de POST /api/auth/login (backend, inglês). */
interface BackendLoginResponse {
  accessToken: string;
  expiresIn: number;
  user: {
    id: string;
    name: string;
    email: string;
    role: string;
    unitId: string;
    unitName: string;
    unitAcronym: string;
    secretariat: string;
  };
}

/**
 * `LoginRequest`/`LoginResponse` continuam com o formato interno (em
 * português) usado pelo resto do app (`useAuthStore`, guards, `Sidebar`,
 * etc.) — a tradução do contrato real do backend (inglês) acontece só aqui,
 * no limite da API, para não precisar tocar em cada consumidor.
 */
export async function login(request: LoginRequest): Promise<LoginResponse> {
  const { data } = await apiClient.post<BackendLoginResponse>(ENDPOINTS.auth.login, {
    email: request.email,
    password: request.senha,
  });

  return {
    token: data.accessToken,
    profissional: {
      id: data.user.id,
      nome: data.user.name,
      email: data.user.email,
      unidadeId: data.user.unitId,
      unidadeSigla: data.user.unitAcronym,
      papelRbac: mapRbacRoleFromApi(data.user.role),
    },
  };
}
