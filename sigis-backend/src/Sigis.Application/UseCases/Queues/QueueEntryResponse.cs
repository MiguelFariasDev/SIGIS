namespace Sigis.Application.UseCases.Queues;

/// <summary>Resumo de uma entrada na fila de atendimento de uma unidade, para exibição.</summary>
/// <param name="Id">Identificador da entrada de fila.</param>
/// <param name="PersonId">Identificador da pessoa na fila.</param>
/// <param name="PersonName">Nome completo da pessoa na fila.</param>
/// <param name="UnitId">Identificador da unidade de serviço.</param>
/// <param name="Specialty">Especialidade solicitada.</param>
/// <param name="Priority">Prioridade atual na fila.</param>
/// <param name="Status">Situação atual da entrada de fila.</param>
/// <param name="EnteredAt">Data e hora (UTC) de entrada na fila.</param>
/// <param name="ConsecutiveAbsences">Número de faltas consecutivas registradas.</param>
public sealed record QueueEntryResponse(
    Guid Id,
    Guid PersonId,
    string PersonName,
    Guid UnitId,
    string Specialty,
    string Priority,
    string Status,
    DateTime EnteredAt,
    int ConsecutiveAbsences);
