using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sigis.Api.Contracts.DevelopmentMilestones;
using Sigis.Application.UseCases.Development;
using Sigis.Domain.Abstractions;

namespace Sigis.Api.Controllers;

/// <summary>Controller de marcos de desenvolvimento de uma pessoa (<c>/api/pessoas/{pessoaId}/desenvolvimento</c>).</summary>
[Route("api/pessoas/{pessoaId:guid}/desenvolvimento")]
[Authorize(Policy = "RequireAuthenticated")]
[Produces("application/json")]
public sealed class DevelopmentMilestonesController : ApiControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>Cria a controller de marcos de desenvolvimento.</summary>
    /// <param name="mediator">Despachante MediatR dos casos de uso.</param>
    public DevelopmentMilestonesController(IMediator mediator) => _mediator = mediator;

    /// <summary>Consulta os marcos de desenvolvimento de uma pessoa.</summary>
    /// <param name="pessoaId">Identificador da pessoa.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>200 com os marcos de desenvolvimento, ou 404 quando a pessoa ou o registro não existirem.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(DevelopmentMilestonesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] Guid pessoaId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetDevelopmentMilestonesQuery(pessoaId), cancellationToken);
        return ToHttpResult(result);
    }

    /// <summary>Cria (se ainda não existir) ou atualiza os marcos de desenvolvimento de uma pessoa.</summary>
    /// <param name="pessoaId">Identificador da pessoa.</param>
    /// <param name="request">Dados dos marcos de desenvolvimento.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>200 com o registro atualizado, 404 se a pessoa não existir, ou 400 em caso de regra violada.</returns>
    [HttpPut]
    [ProducesResponseType(typeof(DevelopmentMilestonesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        [FromRoute] Guid pessoaId, [FromBody] UpdateDevelopmentMilestonesRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateDevelopmentMilestonesCommand(
            pessoaId, request.AgeWalkedMonths, request.AgeTalkedMonths, request.LocomotionDifficulty,
            request.CoordinationDifficulty, request.VisualDifficulty, request.HearingDifficulty,
            request.SpeechProblems, request.CommandComprehension, request.CommunicationForm, request.ManualDominance);

        var result = await _mediator.Send(command, cancellationToken);
        return ToHttpResult(result);
    }
}
