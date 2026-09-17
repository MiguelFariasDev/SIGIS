using MediatR;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Interfaces;

namespace Sigis.Application.UseCases.Referrals;

/// <summary>Processa a consulta do histórico de encaminhamentos de uma pessoa.</summary>
public sealed class GetReferralsByPersonQueryHandler
    : IRequestHandler<GetReferralsByPersonQuery, Result<List<ReferralResponse>>>
{
    private readonly IReferralRepository _referralRepository;
    private readonly IServiceUnitRepository _serviceUnitRepository;

    /// <summary>Cria o handler de consulta de encaminhamentos por pessoa.</summary>
    public GetReferralsByPersonQueryHandler(
        IReferralRepository referralRepository, IServiceUnitRepository serviceUnitRepository)
    {
        _referralRepository = referralRepository;
        _serviceUnitRepository = serviceUnitRepository;
    }

    /// <inheritdoc />
    public async Task<Result<List<ReferralResponse>>> Handle(
        GetReferralsByPersonQuery request, CancellationToken cancellationToken)
    {
        var referrals = await _referralRepository.GetByPersonAsync(request.PersonId, cancellationToken);

        var responses = new List<ReferralResponse>();
        foreach (var referral in referrals)
            responses.Add(await ReferralResponseMapper.MapAsync(referral, _serviceUnitRepository, cancellationToken));

        return Result<List<ReferralResponse>>.Success(responses);
    }
}
