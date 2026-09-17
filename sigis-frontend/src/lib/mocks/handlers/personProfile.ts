import { HttpResponse, http } from "msw";
import type { ConcurrentTreatment, CreateConcurrentTreatmentRequest } from "@/lib/types/concurrentTreatment";
import type { UpsertDevelopmentMilestonesRequest } from "@/lib/types/developmentMilestones";
import type { UpsertFamilyCompositionRequest } from "@/lib/types/familyComposition";
import type { CreateLearningDifficultyRequest } from "@/lib/types/learningDifficulties";
import type { GrantConsentRequest, PersonConsent } from "@/lib/types/personConsent";
import type { UpsertClinicalProfileRequest } from "@/lib/types/personClinicalProfile";
import type { CreateSchoolHistoryRequest, SchoolHistory } from "@/lib/types/schoolHistory";
import { db, gerarId } from "../db";

/**
 * Handlers dos sub-recursos de Pessoa introduzidos pelas facetas
 * Saude/Educacao. Cada um espelha um agregado separado do backend —
 * nunca sao gravados dentro do proprio registro de Person.
 */
export const personProfileHandlers = [
  // ---- Perfil clinico (NASF) ----
  http.get("*/api/pessoas/:id/perfil-clinico", ({ params }) => {
    const perfil = db.clinicalProfiles.find((p) => p.personId === params.id);
    if (!perfil) return HttpResponse.json(null, { status: 404 });
    return HttpResponse.json(perfil);
  }),

  http.put("*/api/pessoas/:id/perfil-clinico", async ({ params, request }) => {
    const body = (await request.json()) as UpsertClinicalProfileRequest;
    const agora = new Date().toISOString();
    let perfil = db.clinicalProfiles.find((p) => p.personId === params.id);
    if (perfil) {
      Object.assign(perfil, body, { updatedAt: agora });
    } else {
      perfil = {
        id: gerarId("perfil-clinico"),
        personId: params.id as string,
        ...body,
        createdAt: agora,
        updatedAt: agora,
      };
      db.clinicalProfiles.push(perfil);
    }
    return HttpResponse.json(perfil);
  }),

  // ---- Tratamentos concomitantes ----
  http.get("*/api/pessoas/:id/tratamentos-concomitantes", ({ params }) => {
    return HttpResponse.json(db.concurrentTreatments.filter((t) => t.personId === params.id));
  }),

  http.post("*/api/pessoas/:id/tratamentos-concomitantes", async ({ params, request }) => {
    const body = (await request.json()) as CreateConcurrentTreatmentRequest;
    const agora = new Date().toISOString();
    const novo: ConcurrentTreatment = {
      id: gerarId("tratamento"),
      personId: params.id as string,
      ...body,
      createdAt: agora,
      updatedAt: agora,
    };
    db.concurrentTreatments.push(novo);
    return HttpResponse.json(novo, { status: 201 });
  }),

  http.delete("*/api/pessoas/:id/tratamentos-concomitantes/:treatmentId", ({ params }) => {
    db.concurrentTreatments = db.concurrentTreatments.filter((t) => t.id !== params.treatmentId);
    return new HttpResponse(null, { status: 204 });
  }),

  // ---- Historico escolar ----
  http.get("*/api/pessoas/:id/historico-escolar", ({ params }) => {
    const itens = db.schoolHistories
      .filter((h) => h.personId === params.id)
      .sort((a, b) => b.schoolYear - a.schoolYear);
    return HttpResponse.json(itens);
  }),

  http.post("*/api/pessoas/:id/historico-escolar", async ({ params, request }) => {
    const body = (await request.json()) as CreateSchoolHistoryRequest;
    const novo: SchoolHistory = {
      id: gerarId("hist-escolar"),
      personId: params.id as string,
      ...body,
      createdAt: new Date().toISOString(),
      createdByProfessionalId: "prof-profissional",
    };
    db.schoolHistories.push(novo);
    return HttpResponse.json(novo, { status: 201 });
  }),

  // ---- Composicao familiar ----
  http.get("*/api/pessoas/:id/composicao-familiar", ({ params }) => {
    const item = db.familyCompositions.find((f) => f.personId === params.id);
    if (!item) return HttpResponse.json(null, { status: 404 });
    return HttpResponse.json(item);
  }),

  http.put("*/api/pessoas/:id/composicao-familiar", async ({ params, request }) => {
    const body = (await request.json()) as UpsertFamilyCompositionRequest;
    const agora = new Date().toISOString();
    let item = db.familyCompositions.find((f) => f.personId === params.id);
    if (item) {
      Object.assign(item, body, { updatedAt: agora });
    } else {
      item = { id: gerarId("familia"), personId: params.id as string, ...body, createdAt: agora, updatedAt: agora };
      db.familyCompositions.push(item);
    }
    return HttpResponse.json(item);
  }),

  // ---- Desenvolvimento ----
  http.get("*/api/pessoas/:id/desenvolvimento", ({ params }) => {
    const item = db.developmentMilestones.find((d) => d.personId === params.id);
    if (!item) return HttpResponse.json(null, { status: 404 });
    return HttpResponse.json(item);
  }),

  http.put("*/api/pessoas/:id/desenvolvimento", async ({ params, request }) => {
    const body = (await request.json()) as UpsertDevelopmentMilestonesRequest;
    const agora = new Date().toISOString();
    let item = db.developmentMilestones.find((d) => d.personId === params.id);
    if (item) {
      Object.assign(item, body, { updatedAt: agora });
    } else {
      item = { id: gerarId("desenvolvimento"), personId: params.id as string, ...body, createdAt: agora, updatedAt: agora };
      db.developmentMilestones.push(item);
    }
    return HttpResponse.json(item);
  }),

  // ---- Dificuldades de aprendizagem ----
  http.get("*/api/pessoas/:id/dificuldades-aprendizagem", ({ params }) => {
    return HttpResponse.json(db.learningDifficulties.filter((d) => d.personId === params.id));
  }),

  http.post("*/api/pessoas/:id/dificuldades-aprendizagem", async ({ params, request }) => {
    const body = (await request.json()) as CreateLearningDifficultyRequest;
    const novo = {
      id: gerarId("dificuldade"),
      personId: params.id as string,
      ...body,
      createdAt: new Date().toISOString(),
    };
    db.learningDifficulties.push(novo);
    return HttpResponse.json(novo, { status: 201 });
  }),

  // ---- Consentimentos LGPD ----
  http.get("*/api/pessoas/:id/consentimentos", ({ params }) => {
    return HttpResponse.json(db.consents.filter((c) => c.personId === params.id));
  }),

  http.post("*/api/pessoas/:id/consentimentos", async ({ params, request }) => {
    const body = (await request.json()) as GrantConsentRequest;
    const novo: PersonConsent = {
      id: gerarId("consentimento"),
      personId: params.id as string,
      ...body,
      grantedAt: new Date().toISOString(),
      createdAt: new Date().toISOString(),
    };
    db.consents.push(novo);
    return HttpResponse.json(novo, { status: 201 });
  }),

  http.delete("*/api/pessoas/:id/consentimentos/:consentId", ({ params }) => {
    const consentimento = db.consents.find((c) => c.id === params.consentId);
    if (!consentimento) {
      return HttpResponse.json({ mensagem: "Consentimento nao encontrado." }, { status: 404 });
    }
    consentimento.granted = false;
    consentimento.revokedAt = new Date().toISOString();
    return HttpResponse.json(consentimento);
  }),

  // ---- Solicitacao de acesso cross-secretaria (RF12) ----
  http.post("*/api/pessoas/:id/solicitar-acesso", async ({ params, request }) => {
    const body = (await request.json()) as {
      legalBasisId: string;
      purpose: string;
      justification: string;
    };

    if (!body.justification || body.justification.trim().length < 50) {
      return HttpResponse.json(
        { mensagem: "Justificativa deve ter pelo menos 50 caracteres." },
        { status: 422 },
      );
    }

    const pessoa = db.pessoas.find((p) => p.id === params.id);
    db.auditoria.push({
      id: gerarId("log"),
      personId: params.id as string,
      personName: pessoa?.fullName ?? "",
      profissionalId: "prof-coordenador",
      profissionalNome: "Roberto Carlos Meneses",
      acao: `Solicitacao de acesso cross-secretaria — ${body.purpose}`,
      baseLegal: db.legalBasis.find((l) => l.id === body.legalBasisId)?.article ?? "",
      legalBasisId: body.legalBasisId,
      justificativa: body.justification,
      dataHora: new Date().toISOString(),
      crossUnidade: true,
    });

    return HttpResponse.json({ granted: true }, { status: 201 });
  }),

  // ---- Listagem global de consentimentos (T17 — painel DPO/COORDENADOR) ----
  http.get("*/api/consentimentos", ({ request }) => {
    const url = new URL(request.url);
    const personId = url.searchParams.get("personId");
    const type = url.searchParams.get("type");
    const status = url.searchParams.get("status");
    const dataInicio = url.searchParams.get("dataInicio");
    const dataFim = url.searchParams.get("dataFim");

    let itens = db.consents.slice();
    if (personId) itens = itens.filter((c) => c.personId === personId);
    if (type) itens = itens.filter((c) => c.type === type);
    if (status === "granted") itens = itens.filter((c) => c.granted);
    if (status === "revoked") itens = itens.filter((c) => !c.granted);
    if (dataInicio) itens = itens.filter((c) => c.grantedAt >= dataInicio);
    if (dataFim) itens = itens.filter((c) => c.grantedAt <= dataFim);

    const comNomePessoa = itens.map((c) => ({
      ...c,
      personName: db.pessoas.find((p) => p.id === c.personId)?.fullName ?? "",
    }));

    return HttpResponse.json(
      comNomePessoa.sort((a, b) => new Date(b.grantedAt).getTime() - new Date(a.grantedAt).getTime()),
    );
  }),
];
