using MediatR;
using Sigis.Domain.Abstractions;

namespace Sigis.Application.UseCases.Queues;

/// <summary>Registra o comparecimento ou a falta de uma pessoa em uma entrada de fila.</summary>
/// <param name="QueueEntryId">Identificador da entrada de fila.</param>
/// <param name="Comparecimento">Situação de comparecimento: "COMPARECEU" ou "FALTOU".</param>
public sealed record RegisterQueueAttendanceCommand(Guid QueueEntryId, string Comparecimento)
    : IRequest<Result<QueueEntryResponse>>;
