using MediatR;
using Sigis.Domain.Abstractions;

namespace Sigis.Application.UseCases.Referrals;

/// <summary>Aceita um encaminhamento pendente.</summary>
/// <param name="ReferralId">Identificador do encaminhamento.</param>
public sealed record AcceptReferralCommand(Guid ReferralId) : IRequest<Result<ReferralResponse>>;
