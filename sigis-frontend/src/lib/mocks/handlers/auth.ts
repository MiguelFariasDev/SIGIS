import { HttpResponse, http } from "msw";
import type { LoginRequest, LoginResponse } from "@/lib/types/auth";
import { db } from "../db";

export const authHandlers = [
  http.post("*/api/auth/login", async ({ request }) => {
    const body = (await request.json()) as LoginRequest;
    const profissional = db.profissionais.find(
      (p) => p.email.toLowerCase() === body.email.trim().toLowerCase(),
    );

    if (!profissional || !body.senha) {
      return HttpResponse.json(
        { mensagem: "Credenciais invalidas." },
        { status: 401 },
      );
    }

    const response: LoginResponse = {
      token: `mock-token-${profissional.id}-${Date.now()}`,
      profissional: {
        id: profissional.id,
        nome: profissional.nome,
        email: profissional.email,
        unidadeId: profissional.unidadeId,
        unidadeSigla: profissional.unidadeSigla ?? "",
        papelRbac: profissional.papelRbac,
      },
    };

    return HttpResponse.json(response, { status: 200 });
  }),
];
