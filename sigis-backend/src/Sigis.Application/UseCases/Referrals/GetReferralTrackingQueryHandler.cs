using MediatR;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Abstractions.Errors;
using Sigis.Domain.Interfaces;

namespace Sigis.Application.UseCases.Referrals;

/// <summary>Processa a consulta de rastreio de um encaminhamento.</summary>
public sealed class GetReferralTrackingQueryHandler : IRequestHandler<GetReferralTrackingQuery, Result<ReferralResponse>>
{
    private readonly IReferralRepository _referralRepository;
    private readonly IServiceUnitRepository _serviceUnitRepository;

    /// <summary>Cria o handler de consulta de rastreio de encaminhamento.</summary>
    public GetReferralTrackingQueryHandler(
        IReferralRepository referralRepository, IServiceUnitRepository serviceUnitRepository)
    {
        _referralRepository = referralRepository;
        _serviceUnitRepository = serviceUnitRepository;
    }

    /// <inheritdoc />
    public async Task<Result<ReferralResponse>> Handle(GetReferralTrackingQuery request, CancellationToken cancellationToken)
    {
        var referral = await _referralRepository.GetByIdAsync(request.ReferralId, cancellationToken);
        if (referral is null)
            return Result<ReferralResponse>.Failure(CommonErrors.NotFound);

        return Result<ReferralResponse>.Success(
            await ReferralResponseMapper.MapAsync(referral, _serviceUnitRepository, cancellationToken));
    }
}
