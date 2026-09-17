using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sigis.Api.Contracts.LearningDifficulties;
using Sigis.Application.UseCases.Learning;
using Sigis.Domain.Abstractions;

namespace Sigis.Api.Controllers;

/// <summary>Controller de dificuldades de aprendizagem de uma pessoa (<c>/api/pessoas/{pessoaId}/dificuldades-aprendizagem</c>).</summary>
[Route("api/pessoas/{pessoaId:guid}/dificuldades-aprendizagem")]
[Authorize(Policy = "RequireAuthenticated")]
[Produces("application/json")]
public sealed class LearningDifficultiesController : ApiControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>Cria a controller de dificuldades de aprendizagem.</summary>
    /// <param name="mediator">Despachante MediatR dos casos de uso.</param>
    public LearningDifficultiesController(IMediator mediator) => _mediator = mediator;

    /// <summary>Lista as dificuldades de aprendizagem registradas para uma pessoa.</summary>
    /// <param name="pessoaId">Identificador da pessoa.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>200 com a lista de dificuldades, ou 404 quando a pessoa não existir.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<LearningDifficultyResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] Guid pessoaId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetLearningDifficultiesQuery(pessoaId), cancellationToken);
        return ToHttpResult(result);
    }

    /// <summary>Registra uma dificuldade de aprendizagem para a pessoa.</summary>
    /// <param name="pessoaId">Identificador da pessoa.</param>
    /// <param name="request">Dados da dificuldade de aprendizagem.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>201 com o registro criado, 404 se a pessoa não existir, ou 409 se já houver registro do mesmo tipo.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(LearningDifficultyResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Add(
        [FromRoute] Guid pessoaId, [FromBody] AddLearningDifficultyRequest request, CancellationToken cancellationToken)
    {
        var command = new AddLearningDifficultyCommand(pessoaId, request.Type, request.Severity, request.AssessmentDate, request.Notes);
        var result = await _mediator.Send(command, cancellationToken);
        return ToCreatedResult(result);
    }
}
