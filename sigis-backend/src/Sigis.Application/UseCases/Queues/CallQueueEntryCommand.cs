using MediatR;
using Sigis.Domain.Abstractions;

namespace Sigis.Application.UseCases.Queues;

/// <summary>Chama a próxima pessoa (ou uma pessoa específica) da fila para atendimento.</summary>
/// <param name="QueueEntryId">Identificador da entrada de fila a chamar.</param>
public sealed record CallQueueEntryCommand(Guid QueueEntryId) : IRequest<Result<QueueEntryResponse>>;
