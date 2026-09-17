import { HttpResponse, http } from "msw";
import { StatusEncaminhamento } from "@/lib/types/enums";
import type { CriarEncaminhamentoRequest, Encaminhamento, RecusarEncaminhamentoRequest } from "@/lib/types/encaminhamento";
import { db, gerarId } from "../db";

export const encaminhamentosHandlers = [
  // Encaminhamentos pendentes de decisao PARA a unidade do profissional logado (T13).
  // Em um backend real isso viria do token JWT; no mock aceitamos ?unidadeDestinoId=.
  http.get("*/api/encaminhamentos/recebidos", ({ request }) => {
    const url = new URL(request.url);
    const unidadeDestinoId = url.searchParams.get("unidadeDestinoId");
    let itens = db.encaminhamentos.filter((e) => e.status === StatusEncaminhamento.PENDENTE);
    if (unidadeDestinoId) itens = itens.filter((e) => e.unidadeDestinoId === unidadeDestinoId);
    return HttpResponse.json(itens);
  }),

  http.get("*/api/encaminhamentos/enviados", ({ request }) => {
    const url = new URL(request.url);
    const unidadeOrigemId = url.searchParams.get("unidadeOrigemId");
    let itens = db.encaminhamentos.slice();
    if (unidadeOrigemId) itens = itens.filter((e) => e.unidadeOrigemId === unidadeOrigemId);
    return HttpResponse.json(itens);
  }),

  http.get("*/api/encaminhamentos/pessoa/:personId", ({ params }) => {
    const itens = db.encaminhamentos
      .filter((e) => e.personId === params.personId)
      .sort((a, b) => new Date(b.dataEncaminhamento).getTime() - new Date(a.dataEncaminhamento).getTime());
    return HttpResponse.json(itens);
  }),

  http.get("*/api/encaminhamentos/:id", ({ params }) => {
    const item = db.encaminhamentos.find((e) => e.id === params.id);
    if (!item) {
      return HttpResponse.json({ mensagem: "Encaminhamento nao encontrado." }, { status: 404 });
    }
    return HttpResponse.json(item);
  }),

  http.post("*/api/encaminhamentos", async ({ request }) => {
    const body = (await request.json()) as CriarEncaminhamentoRequest;
    const pessoa = db.pessoas.find((p) => p.id === body.personId);
    const origem = db.unidades.find((u) => u.id === body.unidadeOrigemId);
    const destino = db.unidades.find((u) => u.id === body.unidadeDestinoId);

    const novo: Encaminhamento = {
      id: gerarId("encaminhamento"),
      personId: body.personId,
      personName: pessoa?.fullName ?? "",
      unidadeOrigemId: body.unidadeOrigemId,
      unidadeOrigemSigla: origem?.sigla ?? "",
      unidadeDestinoId: body.unidadeDestinoId,
      unidadeDestinoSigla: destino?.sigla ?? "",
      motivo: body.motivo,
      prioridade: body.prioridade,
      dataEncaminhamento: new Date().toISOString(),
      status: StatusEncaminhamento.PENDENTE,
      legalBasisId: body.legalBasisId,
    };

    db.encaminhamentos.push(novo);
    return HttpResponse.json(novo, { status: 201 });
  }),

  http.post("*/api/encaminhamentos/:id/aceitar", ({ params }) => {
    const item = db.encaminhamentos.find((e) => e.id === params.id);
    if (!item) {
      return HttpResponse.json({ mensagem: "Encaminhamento nao encontrado." }, { status: 404 });
    }
    item.status = StatusEncaminhamento.ACEITO;
    item.aceitoPor = "Profissional autenticado";
    item.aceitoEm = new Date().toISOString();
    return HttpResponse.json(item);
  }),

  http.post("*/api/encaminhamentos/:id/recusar", async ({ params, request }) => {
    const item = db.encaminhamentos.find((e) => e.id === params.id);
    if (!item) {
      return HttpResponse.json({ mensagem: "Encaminhamento nao encontrado." }, { status: 404 });
    }
    const body = (await request.json()) as RecusarEncaminhamentoRequest;
    if (!body.motivo || body.motivo.trim().length === 0) {
      return HttpResponse.json({ mensagem: "Motivo da recusa e obrigatorio." }, { status: 422 });
    }
    item.status = StatusEncaminhamento.RECUSADO;
    item.recusaMotivo = body.motivo;
    return HttpResponse.json(item);
  }),
];
