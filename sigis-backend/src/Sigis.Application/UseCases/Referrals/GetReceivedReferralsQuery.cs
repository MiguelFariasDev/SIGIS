using MediatR;
using Sigis.Domain.Abstractions;

namespace Sigis.Application.UseCases.Referrals;

/// <summary>Lista os encaminhamentos pendentes ou aceitos direcionados à unidade do profissional autenticado.</summary>
public sealed record GetReceivedReferralsQuery : IRequest<Result<List<ReferralResponse>>>;
