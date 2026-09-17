using MediatR;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Abstractions.Errors;
using Sigis.Domain.Interfaces;

namespace Sigis.Application.UseCases.Referrals;

/// <summary>
/// Processa a consulta de encaminhamentos recebidos pela unidade do
/// profissional autenticado.
/// </summary>
public sealed class GetReceivedReferralsQueryHandler
    : IRequestHandler<GetReceivedReferralsQuery, Result<List<ReferralResponse>>>
{
    private readonly IReferralRepository _referralRepository;
    private readonly IServiceUnitRepository _serviceUnitRepository;
    private readonly ICurrentUserService _currentUserService;

    /// <summary>Cria o handler de consulta de encaminhamentos recebidos.</summary>
    public GetReceivedReferralsQueryHandler(
        IReferralRepository referralRepository,
        IServiceUnitRepository serviceUnitRepository,
        ICurrentUserService currentUserService)
    {
        _referralRepository = referralRepository;
        _serviceUnitRepository = serviceUnitRepository;
        _currentUserService = currentUserService;
    }

    /// <inheritdoc />
    public async Task<Result<List<ReferralResponse>>> Handle(
        GetReceivedReferralsQuery request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.UnitId.HasValue)
            return Result<List<ReferralResponse>>.Failure(AuthErrors.Forbidden);

        var referrals = await _referralRepository.GetActiveByDestinationUnitAsync(
            _currentUserService.UnitId.Value, cancellationToken);

        var responses = new List<ReferralResponse>();
        foreach (var referral in referrals)
            responses.Add(await ReferralResponseMapper.MapAsync(referral, _serviceUnitRepository, cancellationToken));

        return Result<List<ReferralResponse>>.Success(responses);
    }
}
