using MediatR;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Abstractions.Errors;
using Sigis.Domain.Interfaces;

namespace Sigis.Application.UseCases.Consents;

/// <summary>Processa a revogação de um consentimento LGPD já concedido a uma pessoa.</summary>
public sealed class RevokeConsentCommandHandler : IRequestHandler<RevokeConsentCommand, Result>
{
    private readonly IPersonConsentRepository _personConsentRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Cria o handler de revogação de consentimento.</summary>
    public RevokeConsentCommandHandler(
        IPersonConsentRepository personConsentRepository, IDateTimeProvider dateTimeProvider, IUnitOfWork unitOfWork)
    {
        _personConsentRepository = personConsentRepository;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result> Handle(RevokeConsentCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Justification))
            return Result.Failure(new Error("CONSENT_VALIDATION", "Justificativa é obrigatória para revogar um consentimento."));

        var consent = await _personConsentRepository.GetByIdAsync(request.ConsentId, cancellationToken);
        if (consent is null || consent.PersonId != request.PersonId)
            return Result.Failure(CommonErrors.NotFound);

        var revokeResult = consent.Revoke(_dateTimeProvider.UtcNow);
        if (revokeResult.IsFailure)
            return revokeResult;

        await _personConsentRepository.UpdateAsync(consent, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
