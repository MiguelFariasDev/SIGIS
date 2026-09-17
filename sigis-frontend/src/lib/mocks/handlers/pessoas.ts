import { HttpResponse, http } from "msw";
import type { CreatePersonRequest, Person, PersonSearchResult } from "@/lib/types/person";
import type { TimelineEvento } from "@/lib/types/timeline";
import { SERVICO_SECRETARIAT, TIPO_SESSAO_LABEL, type ServicoSigla } from "@/lib/utils/constants";
import { buscarCandidatosDuplicidade, db, gerarId } from "../db";

function paraResultadoBusca(pessoa: Person): PersonSearchResult {
  const filaAtiva = db.filas.find((f) => f.personId === pessoa.id);
  return {
    id: pessoa.id,
    fullName: pessoa.fullName,
    birthDate: pessoa.birthDate,
    motherName: pessoa.motherName,
    services: pessoa.services ?? [],
    queueStatus: filaAtiva?.status,
    priority: filaAtiva?.prioridade,
  };
}

/**
 * Monta a linha do tempo unificada da pessoa. Eventos cuja secretaria de
 * origem difira da secretaria "de origem" da pessoa (primeiro servico
 * cadastrado) sao marcados como cross-secretaria — o profissional
 * autenticado ve o cadeado (RF12) ate solicitar acesso (T15).
 *
 * `nivelAcesso` simula o resultado de uma solicitacao de acesso ja
 * aprovada: quando presente, eventos daquela secretaria deixam de ser
 * bloqueados nesta resposta.
 */
function montarTimeline(pessoa: Person, secretariasLiberadas: Set<string>): TimelineEvento[] {
  const eventos: TimelineEvento[] = [];
  const servicoOrigem = pessoa.services?.[0] as ServicoSigla | undefined;
  const secretariaOrigem = servicoOrigem ? SERVICO_SECRETARIAT[servicoOrigem] : "Health";

  const estaLiberado = (secretariat: string) => secretariat === secretariaOrigem || secretariasLiberadas.has(secretariat);

  eventos.push({
    id: `timeline-cadastro-${pessoa.id}`,
    tipo: "CADASTRO",
    titulo: "Cadastro realizado",
    descricao: "Pessoa incluida no cadastro unico da rede.",
    dataHora: pessoa.createdAt,
    unidadeSigla: servicoOrigem,
    secretariat: secretariaOrigem,
  });

  for (const atendimento of db.atendimentos.filter((a) => a.personId === pessoa.id)) {
    const secretariat = SERVICO_SECRETARIAT[atendimento.unitCode as ServicoSigla] ?? "Health";
    const locked = !estaLiberado(secretariat);
    eventos.push({
      id: `timeline-atendimento-${atendimento.id}`,
      tipo: atendimento.attendanceStatus === "FALTOU" ? "FALTA" : "ATENDIMENTO",
      titulo: locked ? "Atendimento em outra secretaria" : TIPO_SESSAO_LABEL[atendimento.sessionType],
      descricao: locked ? undefined : `Sessao ${atendimento.sessionNumber} — ${atendimento.professionalName}`,
      dataHora: atendimento.dateTime,
      unidadeSigla: atendimento.unitCode,
      profissionalNome: locked ? undefined : atendimento.professionalName,
      acessoRegistrado: secretariat !== secretariaOrigem,
      secretariat,
      locked,
    });
  }

  for (const fila of db.filas.filter((f) => f.personId === pessoa.id)) {
    const unidade = db.unidades.find((u) => u.id === fila.unidadeId);
    const secretariat = unidade ? SERVICO_SECRETARIAT[unidade.sigla as ServicoSigla] ?? "Health" : "Health";
    const locked = !estaLiberado(secretariat);
    eventos.push({
      id: `timeline-fila-${fila.id}`,
      tipo: "ENTRADA_FILA",
      titulo: locked ? "Entrada em fila de outra secretaria" : `Entrada na fila — ${fila.especialidade}`,
      descricao: locked ? undefined : `Prioridade: ${fila.prioridade}`,
      dataHora: fila.entradaFila,
      unidadeSigla: unidade?.sigla,
      secretariat,
      locked,
    });
  }

  for (const encaminhamento of db.encaminhamentos.filter((e) => e.personId === pessoa.id)) {
    const secretariat = SERVICO_SECRETARIAT[encaminhamento.unidadeDestinoSigla as ServicoSigla] ?? "Health";
    eventos.push({
      id: `timeline-encaminhamento-${encaminhamento.id}`,
      tipo: "ENCAMINHAMENTO_ENVIADO",
      titulo: `Encaminhado de ${encaminhamento.unidadeOrigemSigla} para ${encaminhamento.unidadeDestinoSigla}`,
      descricao: encaminhamento.motivo,
      dataHora: encaminhamento.dataEncaminhamento,
      unidadeSigla: encaminhamento.unidadeOrigemSigla,
      acessoRegistrado: true,
      secretariat,
    });
  }

  for (const log of db.auditoria.filter((l) => l.personId === pessoa.id && l.crossUnidade)) {
    eventos.push({
      id: `timeline-acesso-${log.id}`,
      tipo: "ACESSO_CROSS_UNIDADE",
      titulo: "Acesso cross-secretaria registrado",
      descricao: `${log.acao} — ${log.profissionalNome}`,
      dataHora: log.dataHora,
      profissionalNome: log.profissionalNome,
      acessoRegistrado: true,
      secretariat: log.secretariat ?? secretariaOrigem,
    });
  }

  return eventos.sort((a, b) => new Date(b.dataHora).getTime() - new Date(a.dataHora).getTime());
}

export const pessoasHandlers = [
  http.get("*/api/pessoas/buscar", ({ request }) => {
    const url = new URL(request.url);
    const termo = url.searchParams.get("termo")?.trim().toLowerCase() ?? "";
    const servico = url.searchParams.get("servico") ?? undefined;
    const status = url.searchParams.get("status") ?? undefined;
    const prioridade = url.searchParams.get("prioridade") ?? undefined;

    let resultado = db.pessoas.slice();

    if (termo) {
      resultado = resultado.filter((p) => {
        const alvo = `${p.fullName} ${p.cns ?? ""} ${p.cpf ?? ""}`.toLowerCase();
        return alvo.includes(termo);
      });
    }

    if (servico) {
      resultado = resultado.filter((p) => p.services?.includes(servico));
    }

    let convertido = resultado.map(paraResultadoBusca);

    if (status) {
      convertido = convertido.filter((p) => p.queueStatus === status);
    }
    if (prioridade) {
      convertido = convertido.filter((p) => p.priority === prioridade);
    }

    return HttpResponse.json(convertido);
  }),

  http.get("*/api/pessoas/:id/linha-do-tempo", ({ params, request }) => {
    const pessoa = db.pessoas.find((p) => p.id === params.id);
    if (!pessoa) {
      return HttpResponse.json({ mensagem: "Pessoa nao encontrada." }, { status: 404 });
    }
    const url = new URL(request.url);
    const nivel = url.searchParams.get("nivel");
    const liberadas = new Set(nivel ? nivel.split(",") : []);
    const eventos = montarTimeline(pessoa, liberadas);
    return HttpResponse.json({ eventos, hiddenCount: eventos.filter((e) => e.locked).length });
  }),

  // Alias mantido por compatibilidade com integracoes existentes.
  http.get("*/api/pessoas/:id/timeline", ({ params }) => {
    const pessoa = db.pessoas.find((p) => p.id === params.id);
    if (!pessoa) {
      return HttpResponse.json({ mensagem: "Pessoa nao encontrada." }, { status: 404 });
    }
    return HttpResponse.json(montarTimeline(pessoa, new Set()));
  }),

  http.get("*/api/pessoas/:id", ({ params }) => {
    const pessoa = db.pessoas.find((p) => p.id === params.id);
    if (!pessoa) {
      return HttpResponse.json({ mensagem: "Pessoa nao encontrada." }, { status: 404 });
    }
    return HttpResponse.json(pessoa);
  }),

  http.post("*/api/pessoas", async ({ request }) => {
    const body = (await request.json()) as CreatePersonRequest;

    // O backend real nunca bloqueia a criacao por duplicidade — sempre cria
    // e, se achar candidatos por nome+data de nascimento, devolve 201 com
    // `candidates` preenchido (ver comentario em features/pacientes/api.ts).
    const candidatos = buscarCandidatosDuplicidade(body.fullName, body.birthDate);

    const agora = new Date().toISOString();
    const endereco = body.address
      ? [body.address.street, body.address.number, body.address.neighborhood, body.address.city]
          .filter(Boolean)
          .join(", ") + (body.address.state ? ` - ${body.address.state}` : "")
      : undefined;
    const novaPessoa: Person = {
      id: gerarId("pessoa"),
      fullName: body.fullName,
      birthDate: body.birthDate,
      cns: body.cns,
      cpf: body.cpf,
      motherName: body.motherName,
      gender: body.gender,
      raceColor: body.raceColor,
      phone: body.phone,
      email: body.email,
      address: endereco,
      naturality: body.naturality,
      currentSchool: body.currentSchool,
      grade: body.grade,
      shift: body.shift,
      classGroup: body.classGroup,
      zone: body.zone,
      schoolEnrollment: body.schoolEnrollment,
      referredBySchool: body.referredBySchool,
      needsSpecialEducation: body.needsSpecialEducation,
      attendsTutoring: body.attendsTutoring,
      hasFailedGrade: body.hasFailedGrade,
      disabilityTypes: body.disabilityTypes,
      createdAt: agora,
      updatedAt: agora,
      services: [],
    };

    db.pessoas.push(novaPessoa);

    if (body.guardian) {
      db.guardians.push({
        id: gerarId("responsavel"),
        personId: novaPessoa.id,
        name: body.guardian.name,
        cns: body.guardian.cns,
        birthDate: body.guardian.birthDate,
        relationship: body.guardian.relationship,
      });
    }

    return HttpResponse.json(
      {
        personId: novaPessoa.id,
        candidates: candidatos.map((c) => ({
          id: c.pessoa.id,
          name: c.pessoa.fullName,
          birthDate: c.pessoa.birthDate,
          motherName: c.pessoa.motherName,
        })),
      },
      { status: 201 },
    );
  }),
];
