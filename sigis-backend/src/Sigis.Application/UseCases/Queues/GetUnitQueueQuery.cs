using MediatR;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Enums;

namespace Sigis.Application.UseCases.Queues;

/// <summary>Consulta a fila de atendimento de uma unidade, com filtros opcionais.</summary>
/// <param name="UnitId">Identificador da unidade de serviço.</param>
/// <param name="Status">Situação da fila, opcional.</param>
/// <param name="Priority">Prioridade na fila, opcional.</param>
/// <param name="Specialty">Especialidade solicitada, opcional (busca parcial, sem diferenciar maiúsculas/minúsculas).</param>
public sealed record GetUnitQueueQuery(
    Guid UnitId, QueueStatus? Status, QueuePriority? Priority, string? Specialty)
    : IRequest<Result<List<QueueEntryResponse>>>;
