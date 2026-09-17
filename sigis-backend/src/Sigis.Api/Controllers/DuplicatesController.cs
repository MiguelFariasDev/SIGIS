using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sigis.Api.Contracts.Duplicates;
using Sigis.Application.UseCases.Duplicates;
using Sigis.Domain.Abstractions;

namespace Sigis.Api.Controllers;

/// <summary>
/// Controller de Deduplicação (<c>/api/duplicidades</c>): fila de alertas
/// pendentes e resolução como falso positivo (RF04). A confirmação de
/// mesclagem fica em <see cref="PersonsController.Merge"/>, junto ao
/// caso de uso que efetivamente move os registros.
/// </summary>
[Route("api/duplicidades")]
[Authorize]
[Produces("application/json")]
public sealed class DuplicatesController : ApiControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>Cria a controller de duplicidades.</summary>
    /// <param name="mediator">Despachante MediatR dos casos de uso.</param>
    public DuplicatesController(IMediator mediator) => _mediator = mediator;

    /// <summary>Lista os alertas de duplicidade pendentes de revisão (RF04), paginados.</summary>
    /// <param name="skip">Quantidade de registros a pular (padrão 0).</param>
    /// <param name="take">Quantidade máxima de registros retornados (padrão 20).</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>200 OK com a lista de alertas pendentes, podendo ser vazia.</returns>
    [HttpGet("pendentes")]
    [Authorize(Policy = "RequireCoordinator")]
    [ProducesResponseType(typeof(IReadOnlyList<DuplicateAlertResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPending([FromQuery] int skip, [FromQuery] int take, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetPendingDuplicatesQuery(skip, take), cancellationToken);
        return ToHttpResult(result);
    }

    /// <summary>
    /// Marca um alerta de duplicidade como falso positivo — os dois
    /// cadastros são de pessoas diferentes e permanecem separados.
    /// </summary>
    /// <param name="id">Identificador do alerta de duplicidade.</param>
    /// <param name="request">
    /// Observação opcional do coordenador — aceita por compatibilidade com o
    /// contrato da API, mas não é persistida: <c>DuplicateAlert</c> não
    /// possui campo de observação hoje.
    /// </param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>204 No Content, 404 quando o alerta não existir, ou 409 quando já tiver sido resolvido.</returns>
    [HttpPost("{id:guid}/falso-positivo")]
    [Authorize(Policy = "RequireCoordinator")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> MarkAsFalsePositive(
        Guid id, [FromBody] MarkAsFalsePositiveRequest? request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new MarkDuplicateAsFalsePositiveCommand(id), cancellationToken);
        return ToHttpResult(result);
    }
}
