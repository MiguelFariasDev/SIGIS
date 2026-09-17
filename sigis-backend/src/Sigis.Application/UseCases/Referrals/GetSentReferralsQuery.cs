using MediatR;
using Sigis.Domain.Abstractions;

namespace Sigis.Application.UseCases.Referrals;

/// <summary>Lista todos os encaminhamentos enviados pela unidade do profissional autenticado.</summary>
public sealed record GetSentReferralsQuery : IRequest<Result<List<ReferralResponse>>>;
