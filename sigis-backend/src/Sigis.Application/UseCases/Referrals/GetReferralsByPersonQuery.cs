using MediatR;
using Sigis.Domain.Abstractions;

namespace Sigis.Application.UseCases.Referrals;

/// <summary>Lista o histórico de encaminhamentos de uma pessoa em todas as unidades da rede.</summary>
/// <param name="PersonId">Identificador da pessoa.</param>
public sealed record GetReferralsByPersonQuery(Guid PersonId) : IRequest<Result<List<ReferralResponse>>>;
