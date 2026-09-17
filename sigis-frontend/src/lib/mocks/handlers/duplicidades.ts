import { HttpResponse, http } from "msw";
import { StatusAlertaDuplicidade } from "@/lib/types/enums";
import type { ResolverDuplicidadeRequest } from "@/lib/types/duplicidade";
import { db } from "../db";

export const duplicidadesHandlers = [
  http.get("*/api/duplicidades", ({ request }) => {
    const url = new URL(request.url);
    const status = url.searchParams.get("status");
    let itens = db.duplicidades.slice();
    if (status) {
      itens = itens.filter((d) => d.status === status);
    }
    itens = itens.sort((a, b) => b.scoreSimilaridade - a.scoreSimilaridade);
    return HttpResponse.json(itens);
  }),

  http.get("*/api/duplicidades/:id", ({ params }) => {
    const item = db.duplicidades.find((d) => d.id === params.id);
    if (!item) {
      return HttpResponse.json({ mensagem: "Alerta de duplicidade nao encontrado." }, { status: 404 });
    }
    return HttpResponse.json(item);
  }),

  http.post("*/api/duplicidades/:id/resolver", async ({ params, request }) => {
    const item = db.duplicidades.find((d) => d.id === params.id);
    if (!item) {
      return HttpResponse.json({ mensagem: "Alerta de duplicidade nao encontrado." }, { status: 404 });
    }
    const body = (await request.json()) as ResolverDuplicidadeRequest;

    item.status =
      body.acao === "MESCLAR" ? StatusAlertaDuplicidade.MESCLADO : StatusAlertaDuplicidade.FALSO_POSITIVO;
    item.resolvidoPorProfissionalNome = "Roberto Carlos Meneses";
    item.resolvidoEm = new Date().toISOString();

    if (body.acao === "MESCLAR") {
      const pessoa2 = db.pessoas.find((p) => p.id === item.personId2);
      const pessoa1 = db.pessoas.find((p) => p.id === item.personId1);
      if (pessoa1 && pessoa2) {
        pessoa1.services = Array.from(new Set([...(pessoa1.services ?? []), ...(pessoa2.services ?? [])]));
        db.pessoas = db.pessoas.filter((p) => p.id !== pessoa2.id);
      }
    }

    return HttpResponse.json(item);
  }),
];
