import { HttpResponse, http } from "msw";
import { db } from "../db";

export const auditoriaHandlers = [
  http.get("*/api/auditoria", ({ request }) => {
    const url = new URL(request.url);
    const personId = url.searchParams.get("personId");
    const profissionalId = url.searchParams.get("profissionalId");
    const acao = url.searchParams.get("acao")?.toLowerCase();
    const dataInicio = url.searchParams.get("dataInicio");
    const dataFim = url.searchParams.get("dataFim");
    const secretariat = url.searchParams.get("secretariat");
    const crossOnly = url.searchParams.get("crossUnidade") === "true";

    let itens = db.auditoria.slice();
    if (personId) itens = itens.filter((l) => l.personId === personId);
    if (profissionalId) itens = itens.filter((l) => l.profissionalId === profissionalId);
    if (acao) itens = itens.filter((l) => l.acao.toLowerCase().includes(acao));
    if (dataInicio) itens = itens.filter((l) => l.dataHora >= dataInicio);
    if (dataFim) itens = itens.filter((l) => l.dataHora <= dataFim);
    if (secretariat) itens = itens.filter((l) => l.secretariat === secretariat);
    if (crossOnly) itens = itens.filter((l) => l.crossUnidade);

    itens = itens.sort((a, b) => new Date(b.dataHora).getTime() - new Date(a.dataHora).getTime());
    return HttpResponse.json(itens);
  }),

  http.get("*/api/auditoria/exportar", () => {
    const cabecalho = "data_hora,profissional,acao,paciente,base_legal,justificativa,secretaria,cross_unidade";
    const linhas = db.auditoria.map((l) =>
      [
        l.dataHora,
        l.profissionalNome,
        l.acao,
        l.personName,
        l.baseLegal,
        l.justificativa ?? "",
        l.secretariat ?? "",
        l.crossUnidade ? "sim" : "nao",
      ]
        .map((campo) => `"${String(campo).replace(/"/g, '""')}"`)
        .join(","),
    );
    const csv = [cabecalho, ...linhas].join("\n");

    return new HttpResponse(csv, {
      status: 200,
      headers: {
        "Content-Type": "text/csv; charset=utf-8",
        "Content-Disposition": 'attachment; filename="auditoria-sigis.csv"',
      },
    });
  }),
];
