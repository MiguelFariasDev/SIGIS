using MediatR;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Abstractions.Errors;
using Sigis.Domain.Interfaces;

namespace Sigis.Application.UseCases.Duplicates;

/// <summary>Processa a marcação de um alerta de duplicidade como falso positivo.</summary>
public sealed class MarkDuplicateAsFalsePositiveCommandHandler : IRequestHandler<MarkDuplicateAsFalsePositiveCommand, Result>
{
    private readonly IDuplicateAlertRepository _duplicateAlertRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Cria o handler de marcação de falso positivo.</summary>
    public MarkDuplicateAsFalsePositiveCommandHandler(
        IDuplicateAlertRepository duplicateAlertRepository,
        ICurrentUserService currentUserService,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _duplicateAlertRepository = duplicateAlertRepository;
        _currentUserService = currentUserService;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result> Handle(MarkDuplicateAsFalsePositiveCommand request, CancellationToken cancellationToken)
    {
        var alert = await _duplicateAlertRepository.GetByIdAsync(request.DuplicateAlertId, cancellationToken);
        if (alert is null)
            return Result.Failure(CommonErrors.NotFound);

        var professionalId = _currentUserService.ProfessionalId ?? Guid.Empty;
        var result = alert.MarkAsFalsePositive(professionalId, _dateTimeProvider.UtcNow);
        if (result.IsFailure)
            return result;

        await _duplicateAlertRepository.UpdateAsync(alert, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
