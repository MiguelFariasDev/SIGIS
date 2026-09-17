using MediatR;
using Sigis.Domain.Abstractions;

namespace Sigis.Application.UseCases.Referrals;

/// <summary>Recusa um encaminhamento pendente.</summary>
/// <param name="ReferralId">Identificador do encaminhamento.</param>
/// <param name="Motivo">Motivo da recusa.</param>
public sealed record RefuseReferralCommand(Guid ReferralId, string? Motivo) : IRequest<Result<ReferralResponse>>;
