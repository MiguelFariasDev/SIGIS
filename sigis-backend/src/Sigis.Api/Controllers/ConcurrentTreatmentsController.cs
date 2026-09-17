using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sigis.Api.Contracts.ConcurrentTreatments;
using Sigis.Application.UseCases.ConcurrentTreatments;
using Sigis.Domain.Abstractions;

namespace Sigis.Api.Controllers;

/// <summary>Controller de atendimentos concomitantes de uma pessoa (<c>/api/pessoas/{pessoaId}/tratamentos-concomitantes</c>).</summary>
[Route("api/pessoas/{pessoaId:guid}/tratamentos-concomitantes")]
[Authorize(Policy = "RequireAuthenticated")]
[Produces("application/json")]
public sealed class ConcurrentTreatmentsController : ApiControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>Cria a controller de atendimentos concomitantes.</summary>
    /// <param name="mediator">Despachante MediatR dos casos de uso.</param>
    public ConcurrentTreatmentsController(IMediator mediator) => _mediator = mediator;

    /// <summary>Lista os atendimentos concomitantes de uma pessoa.</summary>
    /// <param name="pessoaId">Identificador da pessoa.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>200 com a lista de atendimentos, ou 404 quando a pessoa não existir.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ConcurrentTreatmentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] Guid pessoaId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetConcurrentTreatmentsQuery(pessoaId), cancellationToken);
        return ToHttpResult(result);
    }

    /// <summary>Registra um atendimento concomitante para a pessoa.</summary>
    /// <param name="pessoaId">Identificador da pessoa.</param>
    /// <param name="request">Dados do atendimento concomitante.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>201 com o registro criado, 404 se a pessoa não existir, ou 409 em caso de sobreposição de horário.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ConcurrentTreatmentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Add(
        [FromRoute] Guid pessoaId, [FromBody] AddConcurrentTreatmentRequest request, CancellationToken cancellationToken)
    {
        var command = new AddConcurrentTreatmentCommand(
            pessoaId, request.Specialty, request.Location, request.ProfessionalName, request.DayOfWeek,
            request.StartTime, request.EndTime, request.Notes);

        var result = await _mediator.Send(command, cancellationToken);
        return ToCreatedResult(result);
    }

    /// <summary>Remove um atendimento concomitante da pessoa.</summary>
    /// <param name="pessoaId">Identificador da pessoa.</param>
    /// <param name="id">Identificador do atendimento concomitante.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>204 quando removido com sucesso, ou 404 quando não existir.</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remove([FromRoute] Guid pessoaId, [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new RemoveConcurrentTreatmentCommand(pessoaId, id), cancellationToken);
        return ToHttpResult(result);
    }
}
