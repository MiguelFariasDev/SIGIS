import { HttpResponse, http } from "msw";
import { StatusAlertaDuplicidade, StatusComparecimento, StatusEncaminhamento, StatusFila } from "@/lib/types/enums";
import type { AlertaRecente, AtendimentosPorDiaItem, FilaPorServicoItem, Indicadores } from "@/lib/types/indicadores";
import { db } from "../db";

function inicioPeriodo(periodo: string | null): Date {
  const agora = new Date();
  const inicio = new Date(agora);
  if (periodo === "30D") inicio.setDate(agora.getDate() - 30);
  else if (periodo === "7D") inicio.setDate(agora.getDate() - 7);
  else inicio.setHours(0, 0, 0, 0);
  return inicio;
}

export const indicadoresHandlers = [
  http.get("*/api/indicadores", ({ request }) => {
    const url = new URL(request.url);
    const desde = inicioPeriodo(url.searchParams.get("periodo"));

    const atendimentosNoPeriodo = db.atendimentos.filter((a) => new Date(a.dateTime) >= desde).length;
    const faltasNoPeriodo = db.atendimentos.filter(
      (a) => new Date(a.dateTime) >= desde && a.attendanceStatus === StatusComparecimento.FALTOU,
    ).length;
    const encaminhamentosAtivos = db.encaminhamentos.filter(
      (e) => e.status === StatusEncaminhamento.PENDENTE || e.status === StatusEncaminhamento.ACEITO,
    ).length;
    const duplicidadesPendentes = db.duplicidades.filter(
      (d) => d.status === StatusAlertaDuplicidade.PENDENTE,
    ).length;

    const resposta: Indicadores = {
      atendimentosNoPeriodo,
      faltasNoPeriodo,
      encaminhamentosAtivos,
      duplicidadesPendentes,
    };
    return HttpResponse.json(resposta);
  }),

  http.get("*/api/indicadores/fila-por-servico", () => {
    const porUnidade = new Map<string, FilaPorServicoItem>();
    for (const unidade of db.unidades) {
      porUnidade.set(unidade.sigla, { servico: unidade.sigla, aguardando: 0, emAtendimento: 0 });
    }
    for (const item of db.filas) {
      const unidade = db.unidades.find((u) => u.id === item.unidadeId);
      if (!unidade) continue;
      const bucket = porUnidade.get(unidade.sigla);
      if (!bucket) continue;
      if (item.status === StatusFila.AGUARDANDO) bucket.aguardando += 1;
      if (item.status === StatusFila.EM_ATENDIMENTO) bucket.emAtendimento += 1;
    }
    return HttpResponse.json(Array.from(porUnidade.values()));
  }),

  http.get("*/api/indicadores/atendimentos-por-dia", ({ request }) => {
    const url = new URL(request.url);
    const desde = inicioPeriodo(url.searchParams.get("periodo"));
    const porDia = new Map<string, number>();

    for (const atendimento of db.atendimentos) {
      const data = new Date(atendimento.dateTime);
      if (data < desde) continue;
      const chave = data.toISOString().slice(0, 10);
      porDia.set(chave, (porDia.get(chave) ?? 0) + 1);
    }

    const itens: AtendimentosPorDiaItem[] = Array.from(porDia.entries())
      .map(([data, quantidade]) => ({ data, quantidade }))
      .sort((a, b) => a.data.localeCompare(b.data));

    return HttpResponse.json(itens);
  }),

  http.get("*/api/indicadores/alertas-recentes", () => {
    const duplicidades: AlertaRecente[] = db.duplicidades
      .filter((d) => d.status === StatusAlertaDuplicidade.PENDENTE)
      .map((d) => ({
        id: d.id,
        tipo: "DUPLICIDADE",
        descricao: `${d.person1?.fullName ?? "Pessoa"} pode ser a mesma pessoa que ${d.person2?.fullName ?? "outro cadastro"} (${Math.round(d.scoreSimilaridade * 100)}% similar)`,
        dataHora: d.criadoEm,
      }));

    const buscaAtiva: AlertaRecente[] = db.filas
      .filter((f) => f.status === StatusFila.BUSCA_ATIVA)
      .map((f) => ({
        id: f.id,
        tipo: "BUSCA_ATIVA",
        descricao: `${f.personName} em busca ativa apos faltas consecutivas (${f.especialidade})`,
        dataHora: f.entradaFila,
      }));

    const alertas = [...duplicidades, ...buscaAtiva].sort(
      (a, b) => new Date(b.dataHora).getTime() - new Date(a.dataHora).getTime(),
    );

    return HttpResponse.json(alertas);
  }),
];
