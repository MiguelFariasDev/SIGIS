using MediatR;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Abstractions.Errors;
using Sigis.Domain.Interfaces;

namespace Sigis.Application.UseCases.Referrals;

/// <summary>Processa a recusa de um encaminhamento pendente pela unidade de destino.</summary>
public sealed class RefuseReferralCommandHandler : IRequestHandler<RefuseReferralCommand, Result<ReferralResponse>>
{
    private readonly IReferralRepository _referralRepository;
    private readonly IServiceUnitRepository _serviceUnitRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Cria o handler de recusa de encaminhamento.</summary>
    public RefuseReferralCommandHandler(
        IReferralRepository referralRepository, IServiceUnitRepository serviceUnitRepository, IUnitOfWork unitOfWork)
    {
        _referralRepository = referralRepository;
        _serviceUnitRepository = serviceUnitRepository;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result<ReferralResponse>> Handle(RefuseReferralCommand request, CancellationToken cancellationToken)
    {
        var referral = await _referralRepository.GetByIdAsync(request.ReferralId, cancellationToken);
        if (referral is null)
            return Result<ReferralResponse>.Failure(CommonErrors.NotFound);

        var refuseResult = referral.Refuse(request.Motivo);
        if (refuseResult.IsFailure)
            return Result<ReferralResponse>.Failure(refuseResult.Error);

        await _referralRepository.UpdateAsync(referral, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<ReferralResponse>.Success(
            await ReferralResponseMapper.MapAsync(referral, _serviceUnitRepository, cancellationToken));
    }
}
