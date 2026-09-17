using MediatR;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Abstractions.Errors;
using Sigis.Domain.Interfaces;

namespace Sigis.Application.UseCases.Referrals;

/// <summary>Processa a aceitação de um encaminhamento pendente pela unidade de destino.</summary>
public sealed class AcceptReferralCommandHandler : IRequestHandler<AcceptReferralCommand, Result<ReferralResponse>>
{
    private readonly IReferralRepository _referralRepository;
    private readonly IServiceUnitRepository _serviceUnitRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Cria o handler de aceitação de encaminhamento.</summary>
    public AcceptReferralCommandHandler(
        IReferralRepository referralRepository, IServiceUnitRepository serviceUnitRepository, IUnitOfWork unitOfWork)
    {
        _referralRepository = referralRepository;
        _serviceUnitRepository = serviceUnitRepository;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result<ReferralResponse>> Handle(AcceptReferralCommand request, CancellationToken cancellationToken)
    {
        var referral = await _referralRepository.GetByIdAsync(request.ReferralId, cancellationToken);
        if (referral is null)
            return Result<ReferralResponse>.Failure(CommonErrors.NotFound);

        var acceptResult = referral.Accept();
        if (acceptResult.IsFailure)
            return Result<ReferralResponse>.Failure(acceptResult.Error);

        await _referralRepository.UpdateAsync(referral, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<ReferralResponse>.Success(
            await ReferralResponseMapper.MapAsync(referral, _serviceUnitRepository, cancellationToken));
    }
}
