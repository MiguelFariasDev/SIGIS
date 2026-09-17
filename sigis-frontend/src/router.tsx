import { Navigate, createBrowserRouter } from "react-router-dom";
import { AppLayout } from "@/components/layout/AppLayout";
import { AcessosCrossPage } from "@/features/auditoria/AcessosCrossPage";
import { AuditoriaPage } from "@/features/auditoria/AuditoriaPage";
import { LoginPage } from "@/features/auth/LoginPage";
import { RequireAuth, RequireRole } from "@/features/auth/guards";
import { AtendimentoPage } from "@/features/atendimentos/AtendimentoPage";
import { TemplatesPage } from "@/features/admin/TemplatesPage";
import { ConsentimentosPage } from "@/features/consentimentos/ConsentimentosPage";
import { DashboardPage } from "@/features/dashboard/DashboardPage";
import { DuplicidadeAlertPage } from "@/features/duplicidades/DuplicidadeAlertPage";
import { RevisaoPage } from "@/features/duplicidades/RevisaoPage";
import { EncaminhamentoPage } from "@/features/encaminhamentos/EncaminhamentoPage";
import { RastreioPage } from "@/features/encaminhamentos/RastreioPage";
import { RecebidosPage } from "@/features/encaminhamentos/RecebidosPage";
import { EnviadosPage } from "@/features/encaminhamentos/EnviadosPage";
import { BuscaPage } from "@/features/pacientes/BuscaPage";
import { CadastroPage } from "@/features/pacientes/CadastroPage";
import { PerfilPage } from "@/features/pacientes/PerfilPage";
import { FilaPage } from "@/features/filas/FilaPage";
import { NotFoundPage } from "@/features/NotFoundPage";
import { PapelRbac } from "@/lib/types/enums";

export const router = createBrowserRouter([
  { path: "/login", element: <LoginPage /> },
  {
    path: "/",
    element: (
      <RequireAuth>
        <AppLayout />
      </RequireAuth>
    ),
    children: [
      { index: true, element: <Navigate to="/dashboard" replace /> },
      { path: "dashboard", element: <DashboardPage /> },
      { path: "pacientes", element: <BuscaPage /> },
      { path: "pacientes/novo", element: <CadastroPage /> },
      { path: "pacientes/:id", element: <PerfilPage /> },
      {
        path: "duplicidades",
        element: (
          <RequireRole papeis={[PapelRbac.COORDENADOR]}>
            <RevisaoPage />
          </RequireRole>
        ),
      },
      {
        path: "duplicidades/:id",
        element: (
          <RequireRole papeis={[PapelRbac.COORDENADOR]}>
            <DuplicidadeAlertPage />
          </RequireRole>
        ),
      },
      { path: "filas", element: <FilaPage /> },
      { path: "filas/:unidadeId", element: <FilaPage /> },
      { path: "atendimentos/:id", element: <AtendimentoPage /> },
      { path: "encaminhamentos/novo", element: <EncaminhamentoPage /> },
      { path: "encaminhamentos/recebidos", element: <RecebidosPage /> },
      { path: "encaminhamentos/enviados", element: <EnviadosPage /> },
      { path: "encaminhamentos/:id/rastreio", element: <RastreioPage /> },
      {
        path: "consentimentos",
        element: (
          <RequireRole papeis={[PapelRbac.COORDENADOR, PapelRbac.AUDITOR]}>
            <ConsentimentosPage />
          </RequireRole>
        ),
      },
      {
        path: "auditoria",
        element: (
          <RequireRole papeis={[PapelRbac.AUDITOR]}>
            <AuditoriaPage />
          </RequireRole>
        ),
      },
      {
        path: "auditoria/acessos-cross",
        element: (
          <RequireRole papeis={[PapelRbac.AUDITOR]}>
            <AcessosCrossPage />
          </RequireRole>
        ),
      },
      {
        path: "admin/templates",
        element: (
          <RequireRole papeis={[PapelRbac.COORDENADOR]}>
            <TemplatesPage />
          </RequireRole>
        ),
      },
      { path: "404", element: <NotFoundPage /> },
      { path: "*", element: <NotFoundPage /> },
    ],
  },
]);
