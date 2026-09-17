using MediatR;
using Sigis.Domain.Abstractions;

namespace Sigis.Application.UseCases.Referrals;

/// <summary>
/// Consulta o rastreio de um encaminhamento: situação atual, unidades
/// envolvidas e data de criação. O agregado <c>Referral</c> não guarda um
/// registro por transição de estado (apenas a situação atual), então este
/// rastreio reflete o estado corrente, não um histórico completo de datas
/// de aceite/recusa/conclusão.
/// </summary>
/// <param name="ReferralId">Identificador do encaminhamento.</param>
public sealed record GetReferralTrackingQuery(Guid ReferralId) : IRequest<Result<ReferralResponse>>;
