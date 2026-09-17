using MediatR;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Enums;
using Sigis.Domain.Interfaces;

namespace Sigis.Application.UseCases.Indicators;

/// <summary>Processa a consulta da fila agrupada por unidade de serviço.</summary>
public sealed class GetFilaPorServicoQueryHandler
    : IRequestHandler<GetFilaPorServicoQuery, Result<IReadOnlyList<FilaPorServicoResponse>>>
{
    private readonly IQueueEntryRepository _queueEntryRepository;
    private readonly IServiceUnitRepository _serviceUnitRepository;

    /// <summary>Cria o handler de consulta da fila por serviço.</summary>
    public GetFilaPorServicoQueryHandler(
        IQueueEntryRepository queueEntryRepository, IServiceUnitRepository serviceUnitRepository)
    {
        _queueEntryRepository = queueEntryRepository;
        _serviceUnitRepository = serviceUnitRepository;
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<FilaPorServicoResponse>>> Handle(
        GetFilaPorServicoQuery request, CancellationToken cancellationToken)
    {
        var queueEntries = await _queueEntryRepository.SearchAsync(null, null, null, cancellationToken);
        var units = await _serviceUnitRepository.GetAllAsync(cancellationToken);
        var acronymByUnitId = units.ToDictionary(u => u.Id, u => u.Acronym);

        var response = queueEntries
            .GroupBy(q => q.UnitId)
            .Select(g => new FilaPorServicoResponse(
                acronymByUnitId.GetValueOrDefault(g.Key, g.Key.ToString()),
                g.Count(q => q.Status == QueueStatus.Waiting),
                g.Count(q => q.Status == QueueStatus.InAttendance)))
            .ToList();

        return Result<IReadOnlyList<FilaPorServicoResponse>>.Success(response);
    }
}
