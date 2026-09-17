import { HttpResponse, http } from "msw";
import { StatusComparecimento } from "@/lib/types/enums";
import type { Attendance, CreateAttendanceRequest, UpdateAttendanceRequest } from "@/lib/types/attendance";
import { db, gerarId } from "../db";

export const atendimentosHandlers = [
  http.get("*/api/atendimentos/pessoa/:personId", ({ params }) => {
    const itens = db.atendimentos
      .filter((a) => a.personId === params.personId)
      .sort((a, b) => new Date(b.dateTime).getTime() - new Date(a.dateTime).getTime());
    return HttpResponse.json(itens);
  }),

  http.get("*/api/atendimentos/:id", ({ params }) => {
    const item = db.atendimentos.find((a) => a.id === params.id);
    if (!item) {
      return HttpResponse.json({ mensagem: "Atendimento nao encontrado." }, { status: 404 });
    }
    return HttpResponse.json(item);
  }),

  http.post("*/api/atendimentos", async ({ request }) => {
    const body = (await request.json()) as CreateAttendanceRequest;
    const pessoa = db.pessoas.find((p) => p.id === body.personId);
    const unidade = db.unidades.find((u) => u.id === body.unitId);
    const profissional = db.profissionais.find((p) => p.id === body.professionalId);
    const triagem = body.triagedByProfessionalId
      ? db.profissionais.find((p) => p.id === body.triagedByProfessionalId)
      : undefined;
    const sessoesAnteriores = db.atendimentos.filter(
      (a) => a.personId === body.personId && a.unitId === body.unitId,
    ).length;

    const novo: Attendance = {
      id: gerarId("atendimento"),
      personId: body.personId,
      personName: pessoa?.fullName ?? "",
      unitId: body.unitId,
      unitCode: unidade?.sigla ?? "",
      professionalId: body.professionalId,
      professionalName: profissional?.nome ?? "",
      dateTime: body.dateTime,
      sessionType: body.sessionType,
      attendanceStatus: StatusComparecimento.AGENDADO,
      triagedByProfessionalId: body.triagedByProfessionalId,
      triagedByProfessionalName: triagem?.nome,
      mainComplaint: body.mainComplaint,
      internalReferrals: body.internalReferrals ?? [],
      dadosFormulario: {},
      sessionNumber: sessoesAnteriores + 1,
    };

    db.atendimentos.push(novo);
    return HttpResponse.json(novo, { status: 201 });
  }),

  http.patch("*/api/atendimentos/:id", async ({ params, request }) => {
    const item = db.atendimentos.find((a) => a.id === params.id);
    if (!item) {
      return HttpResponse.json({ mensagem: "Atendimento nao encontrado." }, { status: 404 });
    }
    const body = (await request.json()) as UpdateAttendanceRequest;
    item.attendanceStatus = body.attendanceStatus;
    item.dadosFormulario = body.dadosFormulario;
    if (body.triagedByProfessionalId !== undefined) item.triagedByProfessionalId = body.triagedByProfessionalId;
    if (body.mainComplaint !== undefined) item.mainComplaint = body.mainComplaint;
    if (body.internalReferrals !== undefined) item.internalReferrals = body.internalReferrals;
    return HttpResponse.json(item);
  }),
];
