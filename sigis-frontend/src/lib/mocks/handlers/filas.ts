import { HttpResponse, http } from "msw";
import { PrioridadeFila, StatusFila } from "@/lib/types/enums";
import type { AtualizarStatusFilaRequest, CriarFilaRequest, FilaAtendimento } from "@/lib/types/fila";
import { db, gerarId } from "../db";

const ORDEM_PRIORIDADE: Record<PrioridadeFila, number> = {
  [PrioridadeFila.URGENTE]: 0,
  [PrioridadeFila.CURTO_PRAZO]: 1,
  [PrioridadeFila.LISTA_ESPERA]: 2,
};

export const filasHandlers = [
  http.get("*/api/filas/unidade/:unidadeId", ({ params, request }) => {
    const url = new URL(request.url);
    const personId = url.searchParams.get("personId");
    const especialidade = url.searchParams.get("especialidade");

    let itens = db.filas.filter((f) => f.unidadeId === params.unidadeId);
    if (personId) itens = itens.filter((f) => f.personId === personId);
    if (especialidade) itens = itens.filter((f) => f.especialidade === especialidade);

    const ordenados = itens
      .slice()
      .sort((a, b) => ORDEM_PRIORIDADE[a.prioridade] - ORDEM_PRIORIDADE[b.prioridade]);

    return HttpResponse.json(ordenados);
  }),

  // Visao "rede inteira" — usada pelo COORDENADOR (T07 nivel 1).
  http.get("*/api/filas", ({ request }) => {
    const url = new URL(request.url);
    const personId = url.searchParams.get("personId");
    const unidadeId = url.searchParams.get("unidadeId");
    const especialidade = url.searchParams.get("especialidade");

    let itens = db.filas.slice();
    if (personId) itens = itens.filter((f) => f.personId === personId);
    if (unidadeId) itens = itens.filter((f) => f.unidadeId === unidadeId);
    if (especialidade) itens = itens.filter((f) => f.especialidade === especialidade);

    return HttpResponse.json(itens);
  }),

  http.post("*/api/filas/unidade/:unidadeId/chamar-proximo", ({ params }) => {
    const candidato = db.filas
      .filter((f) => f.unidadeId === params.unidadeId && f.status === StatusFila.AGUARDANDO)
      .sort((a, b) => ORDEM_PRIORIDADE[a.prioridade] - ORDEM_PRIORIDADE[b.prioridade])[0];

    if (!candidato) {
      return HttpResponse.json({ mensagem: "Fila vazia." }, { status: 404 });
    }

    candidato.status = StatusFila.EM_ATENDIMENTO;
    return HttpResponse.json(candidato);
  }),

  http.post("*/api/filas", async ({ request }) => {
    const body = (await request.json()) as CriarFilaRequest;
    const pessoa = db.pessoas.find((p) => p.id === body.personId);
    const nova: FilaAtendimento = {
      id: gerarId("fila"),
      personId: body.personId,
      personName: pessoa?.fullName ?? "",
      personBirthDate: pessoa?.birthDate ?? "",
      unidadeId: body.unidadeId,
      especialidade: body.especialidade,
      prioridade: body.prioridade,
      entradaFila: new Date().toISOString(),
      status: StatusFila.AGUARDANDO,
    };
    db.filas.push(nova);
    return HttpResponse.json(nova, { status: 201 });
  }),

  http.patch("*/api/filas/:id", async ({ params, request }) => {
    const item = db.filas.find((f) => f.id === params.id);
    if (!item) {
      return HttpResponse.json({ mensagem: "Item de fila nao encontrado." }, { status: 404 });
    }
    const body = (await request.json()) as AtualizarStatusFilaRequest;
    item.status = body.status;
    return HttpResponse.json(item);
  }),
];
