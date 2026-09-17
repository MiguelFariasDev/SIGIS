using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sigis.Api.Contracts.Queues;
using Sigis.Application.UseCases.Queues;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Enums;

namespace Sigis.Api.Controllers;

/// <summary>Controller da Fila de Atendimento (<c>/api/filas</c> e <c>/api/unidades/{unidadeId}/fila</c>).</summary>
[Authorize(Policy = "RequireAuthenticated")]
[Produces("application/json")]
[Route("api/filas")]
public sealed class QueuesController : ApiControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>Cria a controller de fila de atendimento.</summary>
    /// <param name="mediator">Despachante MediatR dos casos de uso.</param>
    public QueuesController(IMediator mediator) => _mediator = mediator;

    /// <summary>Lista a fila de atendimento de uma unidade, com filtros opcionais.</summary>
    /// <param name="unidadeId">Identificador da unidade de serviço.</param>
    /// <param name="status">Situação da fila, opcional.</param>
    /// <param name="prioridade">Prioridade na fila, opcional.</param>
    /// <param name="especialidade">Especialidade solicitada, opcional.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>200 com a lista de entradas de fila da unidade, ou 404 quando a unidade não existir.</returns>
    [HttpGet("/api/unidades/{unidadeId:guid}/fila")]
    [ProducesResponseType(typeof(List<QueueEntryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUnitQueue(
        Guid unidadeId,
        [FromQuery] QueueStatus? status,
        [FromQuery] QueuePriority? prioridade,
        [FromQuery] string? especialidade,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetUnitQueueQuery(unidadeId, status, prioridade, especialidade), cancellationToken);
        return ToHttpResult(result);
    }

    /// <summary>Chama uma entrada de fila para atendimento.</summary>
    /// <param name="id">Identificador da entrada de fila.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>200 com a entrada de fila atualizada, ou 409 quando a transição de status for inválida.</returns>
    [HttpPost("{id:guid}/chamar")]
    [ProducesResponseType(typeof(QueueEntryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Call(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CallQueueEntryCommand(id), cancellationToken);
        return ToHttpResult(result);
    }

    /// <summary>Registra o comparecimento ou a falta de uma pessoa em uma entrada de fila.</summary>
    /// <param name="id">Identificador da entrada de fila.</param>
    /// <param name="request">Situação de comparecimento.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>200 com a entrada de fila atualizada, ou 409 quando a fila já estiver concluída.</returns>
    [HttpPatch("{id:guid}/comparecimento")]
    [ProducesResponseType(typeof(QueueEntryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegisterAttendance(
        Guid id, [FromBody] RegisterQueueAttendanceRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new RegisterQueueAttendanceCommand(id, request.Comparecimento), cancellationToken);
        return ToHttpResult(result);
    }
}
