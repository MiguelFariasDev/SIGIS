using MediatR;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Abstractions.Errors;
using Sigis.Domain.Interfaces;

namespace Sigis.Application.UseCases.Queues;

/// <summary>
/// Processa a consulta da fila de atendimento de uma unidade, aplicando os
/// filtros opcionais de situação, prioridade e especialidade.
/// </summary>
public sealed class GetUnitQueueQueryHandler : IRequestHandler<GetUnitQueueQuery, Result<List<QueueEntryResponse>>>
{
    private readonly IQueueEntryRepository _queueEntryRepository;
    private readonly IServiceUnitRepository _serviceUnitRepository;
    private readonly IPersonRepository _personRepository;

    /// <summary>Cria o handler de consulta da fila de uma unidade.</summary>
    public GetUnitQueueQueryHandler(
        IQueueEntryRepository queueEntryRepository,
        IServiceUnitRepository serviceUnitRepository,
        IPersonRepository personRepository)
    {
        _queueEntryRepository = queueEntryRepository;
        _serviceUnitRepository = serviceUnitRepository;
        _personRepository = personRepository;
    }

    /// <inheritdoc />
    public async Task<Result<List<QueueEntryResponse>>> Handle(GetUnitQueueQuery request, CancellationToken cancellationToken)
    {
        var unit = await _serviceUnitRepository.GetByIdAsync(request.UnitId, cancellationToken);
        if (unit is null)
            return Result<List<QueueEntryResponse>>.Failure(CommonErrors.NotFound);

        var entries = await _queueEntryRepository.GetByUnitAsync(request.UnitId, cancellationToken);

        var filtered = entries
            .Where(e => request.Status is null || e.Status == request.Status)
            .Where(e => request.Priority is null || e.Priority == request.Priority)
            .Where(e => string.IsNullOrWhiteSpace(request.Specialty)
                || e.Specialty.Contains(request.Specialty, StringComparison.OrdinalIgnoreCase));

        var responses = new List<QueueEntryResponse>();
        foreach (var entry in filtered)
        {
            var person = await _personRepository.GetByIdAsync(entry.PersonId, cancellationToken);
            responses.Add(new QueueEntryResponse(
                entry.Id,
                entry.PersonId,
                person?.Name.Value ?? string.Empty,
                entry.UnitId,
                entry.Specialty,
                entry.Priority.ToString(),
                entry.Status.ToString(),
                entry.EnteredAt,
                entry.ConsecutiveAbsences));
        }

        return Result<List<QueueEntryResponse>>.Success(responses);
    }
}
