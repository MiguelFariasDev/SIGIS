using MediatR;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Enums;
using Sigis.Domain.Interfaces;

namespace Sigis.Application.UseCases.Indicators;

/// <summary>
/// Processa a consulta de alertas recentes — combina duplicidades pendentes
/// (RF04) e entradas de fila em busca ativa (RN04), mais recentes primeiro.
/// </summary>
public sealed class GetAlertasRecentesQueryHandler
    : IRequestHandler<GetAlertasRecentesQuery, Result<IReadOnlyList<AlertaRecenteResponse>>>
{
    private const int MaximoAlertas = 10;

    private readonly IDuplicateAlertRepository _duplicateAlertRepository;
    private readonly IQueueEntryRepository _queueEntryRepository;
    private readonly IPersonRepository _personRepository;

    /// <summary>Cria o handler de consulta de alertas recentes.</summary>
    public GetAlertasRecentesQueryHandler(
        IDuplicateAlertRepository duplicateAlertRepository,
        IQueueEntryRepository queueEntryRepository,
        IPersonRepository personRepository)
    {
        _duplicateAlertRepository = duplicateAlertRepository;
        _queueEntryRepository = queueEntryRepository;
        _personRepository = personRepository;
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<AlertaRecenteResponse>>> Handle(
        GetAlertasRecentesQuery request, CancellationToken cancellationToken)
    {
        var alertas = new List<AlertaRecenteResponse>();

        var duplicidadesPendentes = await _duplicateAlertRepository.GetPendingAsync(cancellationToken);
        foreach (var alerta in duplicidadesPendentes)
        {
            var pessoa1 = await _personRepository.GetByIdAsync(alerta.PersonId1, cancellationToken);
            var pessoa2 = await _personRepository.GetByIdAsync(alerta.PersonId2, cancellationToken);
            alertas.Add(new AlertaRecenteResponse(
                alerta.Id,
                "DUPLICIDADE",
                $"{pessoa1?.Name.Value ?? "Pessoa não encontrada"} × {pessoa2?.Name.Value ?? "Pessoa não encontrada"}",
                alerta.CreatedAt));
        }

        var todasAsEntradas = await _queueEntryRepository.SearchAsync(null, null, null, cancellationToken);
        foreach (var entrada in todasAsEntradas.Where(q => q.Status == QueueStatus.ActiveSearch))
        {
            var pessoa = await _personRepository.GetByIdAsync(entrada.PersonId, cancellationToken);
            alertas.Add(new AlertaRecenteResponse(
                entrada.Id,
                "BUSCA_ATIVA",
                $"{pessoa?.Name.Value ?? "Pessoa não encontrada"} — {entrada.Specialty}",
                entrada.UpdatedAt));
        }

        var response = alertas
            .OrderByDescending(a => a.DataHora)
            .Take(MaximoAlertas)
            .ToList();

        return Result<IReadOnlyList<AlertaRecenteResponse>>.Success(response);
    }
}
