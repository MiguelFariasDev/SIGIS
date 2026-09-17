using MediatR;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Abstractions.Errors;
using Sigis.Domain.Interfaces;

namespace Sigis.Application.UseCases.Queues;

/// <summary>
/// Processa a chamada de uma entrada de fila para atendimento, transicionando
/// de <see cref="Sigis.Domain.Enums.QueueStatus.Waiting"/> para
/// <see cref="Sigis.Domain.Enums.QueueStatus.InAttendance"/>.
/// </summary>
public sealed class CallQueueEntryCommandHandler : IRequestHandler<CallQueueEntryCommand, Result<QueueEntryResponse>>
{
    private readonly IQueueEntryRepository _queueEntryRepository;
    private readonly IPersonRepository _personRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Cria o handler de chamada de entrada de fila.</summary>
    public CallQueueEntryCommandHandler(
        IQueueEntryRepository queueEntryRepository, IPersonRepository personRepository, IUnitOfWork unitOfWork)
    {
        _queueEntryRepository = queueEntryRepository;
        _personRepository = personRepository;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result<QueueEntryResponse>> Handle(CallQueueEntryCommand request, CancellationToken cancellationToken)
    {
        var entry = await _queueEntryRepository.GetByIdAsync(request.QueueEntryId, cancellationToken);
        if (entry is null)
            return Result<QueueEntryResponse>.Failure(CommonErrors.NotFound);

        var callResult = entry.CallForAttendance();
        if (callResult.IsFailure)
            return Result<QueueEntryResponse>.Failure(callResult.Error);

        await _queueEntryRepository.UpdateAsync(entry, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var person = await _personRepository.GetByIdAsync(entry.PersonId, cancellationToken);

        return Result<QueueEntryResponse>.Success(new QueueEntryResponse(
            entry.Id, entry.PersonId, person?.Name.Value ?? string.Empty, entry.UnitId, entry.Specialty,
            entry.Priority.ToString(), entry.Status.ToString(), entry.EnteredAt, entry.ConsecutiveAbsences));
    }
}
