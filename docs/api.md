# SIGIS — Documentação da API

> Gerado a partir da leitura direta do código-fonte de `sigis-backend/src/Sigis.Api`
> (controllers, contracts, `Program.cs`, `ResultExtensions.cs`). Reflete o estado
> real da API, não apenas o planejado. Ver também [system-design.md](./system-design.md)
> (arquitetura), [governanca-dados.md](./governanca-dados.md) (RBAC) e
> [lgpd.md](./lgpd.md) (base legal e auditoria).

## 1. Visão geral

- **Base URL (dev, `dotnet run`):** `http://localhost:5135` (HTTP) /
  `https://localhost:7236` (HTTPS) — ver `Properties/launchSettings.json`.
- **Base URL (Docker Compose, `make up`):** `http://localhost:5000`.
- **Swagger/OpenAPI:** disponível em `/swagger`, **apenas em ambiente
  Development** (`Swashbuckle.AspNetCore`). O botão "Authorize" do Swagger
  espera **só o token puro**, sem o prefixo `Bearer `.
- **Health check:** `GET /health` (sem autenticação).
- **Versão do build:** `GET /api/version` (sem autenticação) — retorna
  `{ name: "SIGIS", version: "0.1.0", environment }`.
- **Formato:** JSON em todas as rotas, exceto exportação de auditoria
  (`text/csv`). Enums são serializados como **string** (não como número).
- **Versionamento de rota:** o pacote `Asp.Versioning.Mvc` está referenciado
  no projeto, mas **não é usado** nas rotas hoje — não há prefixo `v1/` em
  nenhum endpoint.
- **CORS:** política `SigisCors`, libera `http://localhost:5173` e
  `http://localhost:3000`, qualquer método/header.
- **IDs:** todo identificador de recurso é GUID (`{id:guid}`).
- **Paginação:** não há paginação genérica. Só
  `GET /api/duplicidades/pendentes` aceita `skip`/`take`.

## 2. Autenticação e RBAC

Autenticação via **JWT Bearer**, validado pela própria API (sem
Identity Server/Keycloak/gateway — decisão de arquitetura, ver
[system-design.md](./system-design.md#37-o-que-não-incluir-na-arquitetura-durante-o-hackathon)).

1. `POST /api/auth/login` com `{ email, password }` devolve o token.
2. Nas demais chamadas, enviar `Authorization: Bearer <token>`.
3. Claims do token: `sub` (id do profissional), `name`, `email`, `role`,
   `unit_id`. Expiração padrão: 480 minutos (8h) — configurável em
   `Jwt:ExpirationMinutes`.

**Políticas de autorização** usadas nos controllers:

| Política | Exige | Uso |
| --- | --- | --- |
| `RequireAuthenticated` | Qualquer usuário autenticado | Padrão da maioria dos endpoints |
| `RequireCoordinator` | Role `Coordinator` | Merge de cadastros, resolução de duplicidades, busca global de consentimentos |
| `RequireAuditor` | Role `Auditor` | Todo o controller de auditoria |
| `RequireCoordinatorOrAuditor` | Role `Coordinator` ou `Auditor` | Definida, mas sem uso direto identificado nos controllers atuais |

Papéis RBAC (ver [governanca-dados.md](./governanca-dados.md)):
**Professional** (escopo: própria unidade), **Coordinator** (rede completa +
merge/deduplicação), **Auditor** (somente leitura de auditoria).

## 3. Formato de erro padrão

Toda falha de caso de uso (`Result`/`Result<T>` do MediatR) é convertida em
HTTP por `ApiControllerBase` + `Sigis.Api/Extensions/ResultExtensions.cs`.
O corpo de erro tem **apenas dois campos** — o tipo do erro não é
serializado, ele só determina o status HTTP:

```json
{
  "code": "person.duplicate_cns",
  "message": "Já existe uma pessoa cadastrada com este CNS."
}
```

Mapeamento `ErrorType → status HTTP`:

| ErrorType | Status |
| --- | --- |
| `Validation` | 400 |
| `Unauthorized` | 401 |
| `Forbidden` | 403 |
| `NotFound` | 404 |
| `Conflict` | 409 |
| `Internal` | 500 |

Sucesso sem corpo (ex.: `DELETE`) retorna **204 No Content**. Sucesso com
corpo de criação retorna **201 Created**; demais sucessos, **200 OK**.

## 4. Autenticação (`api/auth`)

| Método | Rota | Auth | Request | Resposta 200 | Erros |
| --- | --- | --- | --- | --- | --- |
| POST | `/api/auth/login` | Público (`AllowAnonymous`) | `LoginRequest { email, password }` | `LoginResponse { accessToken, expiresIn, user: LoginUserSummary { id, name, email, role, unitId, unitName, unitAcronym, secretariat } }` | 400, 401, 403 |
| GET | `/api/auth/me` | Autenticado | — | `{ professionalId, name, email, unitId, role }` (extraído das claims do JWT) | 401 |

## 5. Pessoas — Cadastro único (`api/pessoas`)

| Método | Rota | Auth | Request | Resposta | Erros |
| --- | --- | --- | --- | --- | --- |
| POST | `/api/pessoas` | Autenticado | `CreatePersonRequest { name, birthDate, cns?, cpf?, motherName?, gender?, raceColor?, phone?, email?, street?, number?, neighborhood?, city?, state?, zipCode? }` | 201 (sem candidatos a duplicidade) **ou** 200 (com candidatos) — `CreatePersonResponse { personId, candidates: PersonSummary[] }` | 400, 409 (CNS/CPF já existente) |
| GET | `/api/pessoas/busca?termo=&limit=` | Autenticado | — | 200 `PersonSummary[]` (`limit` padrão 20) | — |
| GET | `/api/pessoas/{id}` | Autenticado | — | 200 `PersonDetailResponse` (dados cadastrais completos + `guardians: GuardianSummary[]`) | 404 |
| GET | `/api/pessoas/{id}/linha-do-tempo?nivel=&justificativa=` | Autenticado | — | 200 `TimelineResponse { personId, level, entries: TimelineEntry[] }` | 403 (justificativa obrigatória e ausente), 404 |
| POST | `/api/pessoas/mesclar` | **Coordinator** | `MergePersonsRequest { sourcePersonId, targetPersonId, duplicateAlertId? }` | 200 `MergeResult { targetPersonId, sourcePersonId, attendancesReassigned, referralsReassigned, queueEntriesReassigned, guardiansCopied }` | 400 (origem = destino), 404 |
| POST | `/api/pessoas/{id}/solicitar-acesso` | Autenticado | `SolicitarAcessoRequest { justificativa }` | 200 `PersonDetailResponse` | 400 (sem justificativa), 404 |

`PersonDetailResponse` inclui campos de Saúde e Educação no mesmo registro
(ex.: `currentSchool`, `grade`, `disabilityTypes`, `needsSpecialEducation`),
refletindo a natureza intersetorial do cadastro único.

### 5.1 Ficha ampliada da pessoa (sub-recursos)

Todos aninhados sob `api/pessoas/{pessoaId}/...`, exigem `Autenticado` e
retornam 404 quando a pessoa não existe.

| Método | Rota | Request | Resposta | Erros |
| --- | --- | --- | --- | --- |
| GET / PUT | `.../perfil-clinico` | `UpdateClinicalProfileRequest { medicalRecordNumber?, clinicalHypothesis?, apsReferenceUnitId? }` | `PersonClinicalProfileResponse` | 400, 404, 409 (nº de prontuário duplicado na unidade APS) |
| GET / PUT | `.../composicao-familiar` | `UpdateFamilyCompositionRequest` (pai/mãe, irmãos, gestação — todos opcionais) | `FamilyCompositionResponse` | 400, 404 |
| GET / PUT | `.../desenvolvimento` | `UpdateDevelopmentMilestonesRequest` (marcos motores, dificuldades — todos opcionais) | `DevelopmentMilestonesResponse` | 400, 404 |
| GET / POST | `.../historico-escolar` | `AddSchoolHistoryRequest { schoolName, grade, schoolYear, shift?, classGroup?, startDate?, notes? }` | GET: `SchoolHistoryResponse[]` · POST 201: `SchoolHistoryResponse` | POST: 400, 404, 409 (já há vínculo ativo) |
| GET / POST | `.../dificuldades-aprendizagem` | `AddLearningDifficultyRequest { type, severity?, assessmentDate?, notes? }` | GET: `LearningDifficultyResponse[]` · POST 201: `LearningDifficultyResponse` | POST: 400, 404, 409 (já registrado o mesmo tipo) |
| GET / POST / DELETE | `.../tratamentos-concomitantes` | `AddConcurrentTreatmentRequest { specialty, location, professionalName, dayOfWeek, startTime, endTime, notes? }` | GET: `ConcurrentTreatmentResponse[]` · POST 201: `ConcurrentTreatmentResponse` · DELETE `/{id}`: 204 | POST: 400, 404, 409 (sobreposição de horário) · DELETE: 404 |
| GET / POST / DELETE | `.../consentimentos` | POST: `GrantConsentRequest { type, version, evidence?, grantedByGuardianId? }` · DELETE: `RevokeConsentRequest { justification }` | GET: `PersonConsentResponse[]` · POST 201: `PersonConsentResponse` · DELETE `/{id}`: 204 | POST: 400, 404, 409 (já ativo do mesmo tipo) · DELETE: 400, 404, 409 (já revogado) |

## 6. Fila de atendimento

| Método | Rota | Auth | Request | Resposta | Erros |
| --- | --- | --- | --- | --- | --- |
| GET | `/api/unidades/{unidadeId}/fila?status=&prioridade=&especialidade=` | Autenticado | — | 200 `QueueEntryResponse[]` (`id, personId, personName, unitId, specialty, priority, status, enteredAt, consecutiveAbsences`) | 404 (unidade não existe) |
| POST | `/api/filas/{id}/chamar` | Autenticado | — | 200 `QueueEntryResponse` | 404, 409 (transição de status inválida) |
| PATCH | `/api/filas/{id}/comparecimento` | Autenticado | `RegisterQueueAttendanceRequest { comparecimento: "COMPARECEU"\|"FALTOU" }` | 200 `QueueEntryResponse` | 400, 404, 409 (fila já concluída) |

## 7. Atendimentos (`api/atendimentos`)

| Método | Rota | Auth | Request | Resposta | Erros |
| --- | --- | --- | --- | --- | --- |
| POST | `/api/atendimentos` | Autenticado | `RegisterAttendanceRequest { personId, unitId, professionalId, dateTime, sessionType, formData?, triagedByProfessionalId?, mainComplaint? }` | 201 `AttendanceResponse` | 400, 404 |
| GET | `/api/atendimentos/{id}` | Autenticado | — | 200 `AttendanceResponse` | 404 |
| GET | `/api/atendimentos/pessoa/{pessoaId}` | Autenticado | — | 200 `AttendanceResponse[]` | — |
| PATCH | `/api/atendimentos/{id}/comparecimento` | Autenticado | `RegisterAttendanceComparecimentoRequest { comparecimento, mainComplaint? }` | 200 `AttendanceResponse` | 400, 404, 409 (comparecimento já registrado) |

`AttendanceResponse`: `id, personId, personName, unitId, unitAcronym,
professionalId, professionalName, dateTime, sessionType, sessionNumber,
status, formData?, triagedByProfessionalId?, mainComplaint?, createdAt`.

## 8. Encaminhamentos (`api/encaminhamentos`)

| Método | Rota | Auth | Request | Resposta | Erros |
| --- | --- | --- | --- | --- | --- |
| POST | `/api/encaminhamentos` | Autenticado | `ReferPersonRequest { personId, originUnitId, destinationUnitId, reason?, priority }` | 201 `ReferralResponse` | 400, 403, 404 |
| GET | `/api/encaminhamentos/recebidos` | Autenticado | — (usa a unidade do profissional logado) | 200 `ReferralResponse[]` | 403 |
| GET | `/api/encaminhamentos/enviados` | Autenticado | — | 200 `ReferralResponse[]` | 403 |
| GET | `/api/encaminhamentos/pessoa/{pessoaId}` | Autenticado | — | 200 `ReferralResponse[]` | — |
| POST | `/api/encaminhamentos/{id}/aceitar` | Autenticado | — | 200 `ReferralResponse` | 404, 409 (não está pendente) |
| POST | `/api/encaminhamentos/{id}/recusar` | Autenticado | `RefuseReferralRequest { motivo? }` | 200 `ReferralResponse` | 400, 404, 409 (não está pendente) |
| GET | `/api/encaminhamentos/{id}/rastreio` | Autenticado | — | 200 `ReferralResponse` | 404 |

`ReferralResponse`: `id, personId, originUnitId, originUnitName,
destinationUnitId, destinationUnitName, reason, priority, referralDate,
status, correlationId`.

## 9. Duplicidades (`api/duplicidades`) — restrito a **Coordinator**

| Método | Rota | Request | Resposta | Erros |
| --- | --- | --- | --- | --- |
| GET | `/api/duplicidades/pendentes?skip=&take=` | — | 200 `DuplicateAlertResponse[]` (`skip`/`take` padrão 0/20) | — |
| GET | `/api/duplicidades/{id}` | — | 200 `DuplicateAlertResponse` | 404 |
| POST | `/api/duplicidades/{id}/resolver` | `ResolveDuplicateRequest { acao: "MESCLAR"\|"FALSO_POSITIVO" }` | 200 `DuplicateAlertResponse` | 400, 404, 409 (já resolvido) |
| POST | `/api/duplicidades/{id}/falso-positivo` | `MarkAsFalsePositiveRequest { note? }` | 204 | 404, 409 (já resolvido) |

> A confirmação de mesclagem efetiva do cadastro é feita em
> `POST /api/pessoas/mesclar` (seção 5), não neste controller — este
> controller só administra o ciclo de vida do alerta.

## 10. Auditoria (`api/auditoria`) — restrito a **Auditor**

| Método | Rota | Query | Resposta |
| --- | --- | --- | --- |
| GET | `/api/auditoria/acessos` | `pessoaId?`, `profissionalId?`, `dataInicio?`, `dataFim?` | 200 `AccessLogResponse[]` (`id, personId, personName, professionalId, professionalName, action, legalBasis, justification?, dateTime, isCrossUnit`) |
| GET | `/api/auditoria/acessos-cross` | — | 200 `CrossAccessResponse[]` (acessos que cruzaram unidade/secretaria) |
| GET | `/api/auditoria/export-csv` | mesmos filtros de `/acessos` | 200, arquivo `text/csv` (`auditoria-sigis.csv`) |

## 11. Indicadores (`api/indicadores`)

| Método | Rota | Query | Resposta |
| --- | --- | --- | --- |
| GET | `/api/indicadores/painel` | `unidadeId?`, `dataInicio?`, `dataFim?` | 200 `IndicatorsResponse { totalNaFilaAtiva, filaPorStatus, filaPorPrioridade, tempoMedioEsperaMinutos?, totalAtendimentos, atendimentosPorStatus, totalEncaminhamentos, encaminhamentosPorStatus }` |
| GET | `/api/indicadores/fila-por-servico` | — | 200 `FilaPorServicoResponse[]` (`servico, aguardando, emAtendimento`) |
| GET | `/api/indicadores/atendimentos-por-dia` | `periodo?` (`"HOJE"\|"7D"\|"30D"`) | 200 `AtendimentosPorDiaResponse[]` (`data, quantidade`) |
| GET | `/api/indicadores/alertas-recentes` | — | 200 `AlertaRecenteResponse[]` (`id, tipo: "DUPLICIDADE"\|"BUSCA_ATIVA", descricao, dataHora`) |

## 12. Consentimentos LGPD — busca global (`api/consentimentos`) — restrito a **Coordinator**

| Método | Rota | Query | Resposta |
| --- | --- | --- | --- |
| GET | `/api/consentimentos` | `personId?`, `type?`, `status?` (`"granted"\|"revoked"`), `dataInicio?`, `dataFim?` | 200 `PersonConsentWithPersonResponse[]` (mesmos campos de `PersonConsentResponse` + `personName`) |

> Distinto de `GET/POST/DELETE /api/pessoas/{pessoaId}/consentimentos`
> (seção 5.1), que opera por pessoa. Este é o painel de auditoria/DPO
> que cruza todas as pessoas.

## 13. Unidades de serviço (`api/unidades`)

| Método | Rota | Resposta |
| --- | --- | --- |
| GET | `/api/unidades` | 200 `ServiceUnitResponse[]` (`id, name, acronym, secretariat`) — lista completa, para seletores da UI |

## 14. Bases legais LGPD (`api/legal-basis`) — conteúdo estático

| Método | Rota | Query | Resposta |
| --- | --- | --- | --- |
| GET | `/api/legal-basis?secretariat=` | `secretariat?` (`ResponsibleSecretariat`) | 200 `LegalBasisResponse[]` (`secretariat, article, justification`) — não persiste dado, apenas conteúdo estático informativo |

## 15. Enums usados em request/response

Serializados sempre como string (não como número).

```
QueueStatus:             AGUARDANDO | EM_ATENDIMENTO | CONCLUIDO | FALTOU | BUSCA_ATIVA
QueuePriority:            URGENTE | CURTO_PRAZO | LISTA_ESPERA
SessionType:               ANAMNESE_PSI | ANAMNESE_PSICOPED | INSTRUMENTAL_EDFISICA | PRONTUARIO_NASF | SINTESE
ReferralStatus:            PENDENTE | ACEITO | CONCLUIDO | RECUSADO
DuplicateAlertStatus:      PENDENTE | MESCLADO | FALSO_POSITIVO
ConsentType:                Clinical | Educational | SocialAssistance | Research
ResponsibleSecretariat:    SAUDE | EDUCACAO | ASSISTENCIA
SchoolStatus:               (status do vínculo escolar — ver SchoolHistoryResponse)
LearningDifficultyType:     (tipos de dificuldade de aprendizagem)
DayOfWeek:                  enum customizado em Sigis.Domain.Enums (tratamentos concomitantes)
```

Ver [system-design.md](./system-design.md#6-enums-do-domínio) para os
enums centrais do domínio (fila, atendimento, encaminhamento, RBAC).

## 16. Exemplo de fluxo de autenticação

```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"coordenador@sigis.gov.br","password":"Senha123!"}'

curl http://localhost:5000/api/pessoas/busca?termo=maria \
  -H "Authorization: Bearer <accessToken>"
```

Credenciais de demonstração e detalhes do seed em [../README.md](../README.md#popular-o-banco-com-dados-de-demonstração).
