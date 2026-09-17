import type { ReactNode } from "react";
import { Navigate } from "react-router-dom";
import type { PapelRbac } from "@/lib/types/enums";
import { useAuthStore } from "@/stores/authStore";

export function RequireAuth({ children }: { children: ReactNode }) {
  const token = useAuthStore((s) => s.token);
  if (!token) return <Navigate to="/login" replace />;
  return <>{children}</>;
}

export function RequireRole({ papeis, children }: { papeis: PapelRbac[]; children: ReactNode }) {
  const usuario = useAuthStore((s) => s.usuario);
  if (!usuario || !papeis.includes(usuario.papelRbac)) return <Navigate to="/dashboard" replace />;
  return <>{children}</>;
}
