using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sigis.Api.Contracts.SchoolHistory;
using Sigis.Application.UseCases.SchoolHistories;
using Sigis.Domain.Abstractions;

namespace Sigis.Api.Controllers;

/// <summary>Controller de histórico escolar de uma pessoa (<c>/api/pessoas/{pessoaId}/historico-escolar</c>).</summary>
[Route("api/pessoas/{pessoaId:guid}/historico-escolar")]
[Authorize(Policy = "RequireAuthenticated")]
[Produces("application/json")]
public sealed class SchoolHistoryController : ApiControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>Cria a controller de histórico escolar.</summary>
    /// <param name="mediator">Despachante MediatR dos casos de uso.</param>
    public SchoolHistoryController(IMediator mediator) => _mediator = mediator;

    /// <summary>Lista o histórico escolar completo de uma pessoa.</summary>
    /// <param name="pessoaId">Identificador da pessoa.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>200 com a lista de registros, ou 404 quando a pessoa não existir.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<SchoolHistoryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] Guid pessoaId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetSchoolHistoryQuery(pessoaId), cancellationToken);
        return ToHttpResult(result);
    }

    /// <summary>Registra um novo vínculo escolar para a pessoa.</summary>
    /// <param name="pessoaId">Identificador da pessoa.</param>
    /// <param name="request">Dados do vínculo escolar.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>201 com o registro criado, 404 se a pessoa não existir, ou 409 se já houver um registro ativo.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(SchoolHistoryResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Add(
        [FromRoute] Guid pessoaId, [FromBody] AddSchoolHistoryRequest request, CancellationToken cancellationToken)
    {
        var command = new AddSchoolHistoryCommand(
            pessoaId, request.SchoolName, request.Grade, request.SchoolYear, request.Shift, request.ClassGroup,
            request.StartDate, request.Notes);

        var result = await _mediator.Send(command, cancellationToken);
        return ToCreatedResult(result);
    }
}
