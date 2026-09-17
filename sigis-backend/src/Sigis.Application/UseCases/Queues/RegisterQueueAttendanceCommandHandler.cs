using MediatR;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Abstractions.Errors;
using Sigis.Domain.Interfaces;

namespace Sigis.Application.UseCases.Queues;

/// <summary>
/// Processa o registro de comparecimento ou falta em uma entrada de fila.
/// </summary>
public sealed class RegisterQueueAttendanceCommandHandler
    : IRequestHandler<RegisterQueueAttendanceCommand, Result<QueueEntryResponse>>
{
    private readonly IQueueEntryRepository _queueEntryRepository;
    private readonly IPersonRepository _personRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Cria o handler de registro de comparecimento na fila.</summary>
    public RegisterQueueAttendanceCommandHandler(
        IQueueEntryRepository queueEntryRepository, IPersonRepository personRepository, IUnitOfWork unitOfWork)
    {
        _queueEntryRepository = queueEntryRepository;
        _personRepository = personRepository;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result<QueueEntryResponse>> Handle(
        RegisterQueueAttendanceCommand request, CancellationToken cancellationToken)
    {
        var entry = await _queueEntryRepository.GetByIdAsync(request.QueueEntryId, cancellationToken);
        if (entry is null)
            return Result<QueueEntryResponse>.Failure(CommonErrors.NotFound);

        var registrationResult = request.Comparecimento?.Trim().ToUpperInvariant() switch
        {
            "COMPARECEU" => entry.MarkAsAttended(),
            "FALTOU" => entry.RegisterAbsence(),
            _ => Result.Failure(new Error(
                "QUEUE_VALIDATION", "Comparecimento deve ser \"COMPARECEU\" ou \"FALTOU\".")),
        };

        if (registrationResult.IsFailure)
            return Result<QueueEntryResponse>.Failure(registrationResult.Error);

        await _queueEntryRepository.UpdateAsync(entry, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var person = await _personRepository.GetByIdAsync(entry.PersonId, cancellationToken);

        return Result<QueueEntryResponse>.Success(new QueueEntryResponse(
            entry.Id, entry.PersonId, person?.Name.Value ?? string.Empty, entry.UnitId, entry.Specialty,
            entry.Priority.ToString(), entry.Status.ToString(), entry.EnteredAt, entry.ConsecutiveAbsences));
    }
}
