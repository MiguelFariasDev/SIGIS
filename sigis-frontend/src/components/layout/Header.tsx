import { LogOut, User } from "lucide-react";
import { useLocation } from "react-router-dom";
import { type BreadcrumbEntry, AppBreadcrumb } from "@/components/layout/Breadcrumb";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { PAPEL_LABEL } from "@/lib/utils/constants";
import { useAuthStore } from "@/stores/authStore";

function montarBreadcrumb(pathname: string): BreadcrumbEntry[] {
  if (pathname.startsWith("/dashboard")) return [{ label: "Painel de indicadores" }];
  if (pathname === "/pacientes") return [{ label: "Pacientes" }];
  if (pathname === "/pacientes/novo") return [{ label: "Pacientes", href: "/pacientes" }, { label: "Novo cadastro" }];
  if (/^\/pacientes\/[^/]+$/.test(pathname))
    return [{ label: "Pacientes", href: "/pacientes" }, { label: "Ficha do paciente" }];
  if (pathname === "/duplicidades") return [{ label: "Duplicidades" }];
  if (/^\/duplicidades\/[^/]+$/.test(pathname))
    return [{ label: "Duplicidades", href: "/duplicidades" }, { label: "Revisao de duplicidade" }];
  if (/^\/filas\//.test(pathname)) return [{ label: "Fila de atendimento" }];
  if (/^\/atendimentos\//.test(pathname)) return [{ label: "Atendimento" }];
  if (pathname === "/encaminhamentos/novo") return [{ label: "Novo encaminhamento" }];
  if (/^\/encaminhamentos\/.*\/rastreio$/.test(pathname)) return [{ label: "Rastreio do encaminhamento" }];
  if (pathname === "/auditoria") return [{ label: "Auditoria" }];
  return [];
}

export function Header() {
  const location = useLocation();
  const usuario = useAuthStore((s) => s.usuario);
  const logout = useAuthStore((s) => s.logout);
  const breadcrumbItems = montarBreadcrumb(location.pathname);

  return (
    <header className="flex h-16 shrink-0 items-center justify-between border-b border-border bg-background px-6">
      <AppBreadcrumb items={breadcrumbItems} />

      {usuario && (
        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <button
              type="button"
              className="flex items-center gap-2 rounded-full border border-border px-3 py-1.5 text-sm font-medium text-foreground transition-colors hover:bg-muted"
            >
              <User className="size-4" />
              {usuario.nome.split(" ")[0]}
            </button>
          </DropdownMenuTrigger>
          <DropdownMenuContent align="end">
            <DropdownMenuLabel>
              <p className="font-medium">{usuario.nome}</p>
              <p className="text-xs font-normal text-muted-foreground">
                {PAPEL_LABEL[usuario.papelRbac]} — {usuario.unidadeSigla}
              </p>
            </DropdownMenuLabel>
            <DropdownMenuSeparator />
            <DropdownMenuItem onClick={logout} variant="destructive">
              <LogOut className="size-4" />
              Sair
            </DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>
      )}
    </header>
  );
}
