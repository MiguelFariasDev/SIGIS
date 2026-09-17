import { useQuery } from "@tanstack/react-query";
import {
  AlertTriangle,
  FileStack,
  Inbox,
  LayoutDashboard,
  ListOrdered,
  LogOut,
  Send,
  ShieldCheck,
  Users,
} from "lucide-react";
import type { LucideIcon } from "lucide-react";
import type { ReactNode } from "react";
import { NavLink } from "react-router-dom";
import { Logo } from "@/components/layout/Logo";
import { fetchDuplicidades } from "@/features/duplicidades/api";
import { fetchEncaminhamentosRecebidos } from "@/features/encaminhamentos/api";
import { PapelRbac, StatusAlertaDuplicidade } from "@/lib/types/enums";
import { cn } from "@/lib/utils";
import { useAuthStore } from "@/stores/authStore";

interface NavItem {
  to: string;
  label: string;
  icon: LucideIcon;
  papeisPermitidos?: PapelRbac[];
  badge?: number;
}

function NavLinkItem({ item }: { item: NavItem }) {
  return (
    <NavLink
      to={item.to}
      className={({ isActive }) =>
        cn(
          "flex items-center gap-3 rounded-lg px-3 py-2 text-sm font-medium transition-colors",
          isActive ? "bg-white/15 text-white" : "text-white/80 hover:bg-white/10 hover:text-white",
        )
      }
    >
      <item.icon className="size-4" />
      <span className="flex-1">{item.label}</span>
      {!!item.badge && (
        <span className="rounded-full bg-sus-yellow px-1.5 py-0.5 text-[10px] font-semibold text-sus-yellow-dark">
          {item.badge}
        </span>
      )}
    </NavLink>
  );
}

function NavGroup({ label, children }: { label: string; children: ReactNode }) {
  return (
    <div>
      <p className="px-3 pb-1 pt-3 text-[11px] font-semibold uppercase tracking-wide text-white/50">{label}</p>
      <div className="space-y-1">{children}</div>
    </div>
  );
}

export function Sidebar() {
  const usuario = useAuthStore((s) => s.usuario);
  const logout = useAuthStore((s) => s.logout);

  const duplicidadesQuery = useQuery({
    queryKey: ["duplicidades", "pendentes-count"],
    queryFn: () => fetchDuplicidades(StatusAlertaDuplicidade.PENDENTE),
    // `GET /api/duplicidades/pendentes` real é restrito a Coordinator (RF04) — chamar para os demais papéis só gera 403.
    enabled: !!usuario && usuario.papelRbac === PapelRbac.COORDENADOR,
  });

  const recebidosQuery = useQuery({
    queryKey: ["encaminhamentos", "recebidos-count", usuario?.unidadeId],
    queryFn: () => fetchEncaminhamentosRecebidos(),
    enabled: !!usuario && usuario.papelRbac !== PapelRbac.AUDITOR,
  });

  const podeVerFila = usuario?.papelRbac !== PapelRbac.AUDITOR;
  const podeVerConsentimentos = usuario?.papelRbac === PapelRbac.COORDENADOR || usuario?.papelRbac === PapelRbac.AUDITOR;
  const podeVerAdmin = usuario?.papelRbac === PapelRbac.COORDENADOR;
  const rotaFila = usuario?.papelRbac === PapelRbac.COORDENADOR ? "/filas" : `/filas/${usuario?.unidadeId}`;

  return (
    <aside className="flex h-screen w-64 shrink-0 flex-col overflow-y-auto bg-sidebar text-sidebar-foreground">
      <div className="px-5 py-5">
        <Logo />
      </div>

      {usuario && podeVerFila && (
        <NavLink
          to={rotaFila}
          className={({ isActive }) =>
            cn(
              "mx-3 mb-1 flex items-center gap-3 rounded-lg px-3 py-2 text-sm font-medium transition-colors",
              isActive ? "bg-white/15 text-white" : "text-white/80 hover:bg-white/10 hover:text-white",
            )
          }
        >
          <ListOrdered className="size-4" />
          Fila
        </NavLink>
      )}

      <nav className="flex-1 space-y-1 px-3 pb-3">
        <NavLinkItem item={{ to: "/dashboard", label: "Painel", icon: LayoutDashboard }} />
        <NavLinkItem item={{ to: "/pacientes", label: "Pacientes", icon: Users }} />
        {usuario?.papelRbac === PapelRbac.COORDENADOR && (
          <NavLinkItem
            item={{
              to: "/duplicidades",
              label: "Duplicidades",
              icon: AlertTriangle,
              badge: duplicidadesQuery.data?.length,
            }}
          />
        )}

        <NavGroup label="Encaminhamentos">
          <NavLinkItem
            item={{
              to: "/encaminhamentos/recebidos",
              label: "Recebidos",
              icon: Inbox,
              badge: recebidosQuery.data?.length,
            }}
          />
          <NavLinkItem item={{ to: "/encaminhamentos/enviados", label: "Enviados", icon: Send }} />
        </NavGroup>

        {usuario?.papelRbac === PapelRbac.AUDITOR && (
          <NavGroup label="Auditoria">
            <NavLinkItem item={{ to: "/auditoria", label: "Logs de acesso", icon: ShieldCheck }} />
            <NavLinkItem item={{ to: "/auditoria/acessos-cross", label: "Acessos cross-secretaria", icon: ShieldCheck }} />
          </NavGroup>
        )}

        {podeVerConsentimentos && (
          <NavGroup label="LGPD">
            <NavLinkItem item={{ to: "/consentimentos", label: "Consentimentos", icon: ShieldCheck }} />
          </NavGroup>
        )}

        {podeVerAdmin && (
          <NavGroup label="Admin">
            <NavLinkItem item={{ to: "/admin/templates", label: "Templates", icon: FileStack }} />
          </NavGroup>
        )}
      </nav>

      <div className="border-t border-white/10 p-3">
        {usuario && (
          <div className="mb-2 px-2">
            <p className="truncate text-sm font-medium text-white">{usuario.nome}</p>
            <p className="truncate text-xs text-white/70">{usuario.unidadeSigla}</p>
          </div>
        )}
        <button
          type="button"
          onClick={logout}
          className="flex w-full items-center gap-3 rounded-lg px-3 py-2 text-sm font-medium text-white/80 transition-colors hover:bg-white/10 hover:text-white"
        >
          <LogOut className="size-4" />
          Sair
        </button>
      </div>
    </aside>
  );
}
