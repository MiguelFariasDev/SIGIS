import { create } from "zustand";
import { persist } from "zustand/middleware";
import type { PapelRbac } from "@/lib/types/enums";

export interface UsuarioAutenticado {
  id: string;
  nome: string;
  email: string;
  unidadeId: string;
  unidadeSigla: string;
  papelRbac: PapelRbac;
}

interface AuthState {
  token: string | null;
  usuario: UsuarioAutenticado | null;
  login: (token: string, usuario: UsuarioAutenticado) => void;
  logout: () => void;
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set) => ({
      token: null,
      usuario: null,
      login: (token, usuario) => set({ token, usuario }),
      logout: () => set({ token: null, usuario: null }),
    }),
    { name: "sigis-auth" },
  ),
);

export function isAutenticado(): boolean {
  return useAuthStore.getState().token !== null;
}
