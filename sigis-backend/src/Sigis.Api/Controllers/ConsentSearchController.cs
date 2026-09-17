using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sigis.Application.UseCases.Consents;
using Sigis.Domain.Enums;

namespace Sigis.Api.Controllers;

/// <summary>Controller do painel global de consentimentos LGPD (<c>/api/consentimentos</c>) — DPO/coordenador (T17).</summary>
[Route("api/consentimentos")]
[Produces("application/json")]
[Authorize(Policy = "RequireCoordinator")]
public sealed class ConsentSearchController : ApiControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>Cria a controller de busca global de consentimentos.</summary>
    /// <param name="mediator">Despachante MediatR dos casos de uso.</param>
    public ConsentSearchController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Lista consentimentos LGPD de todas as pessoas, filtrados
    /// opcionalmente por pessoa, tipo, situação e período.
    /// </summary>
    /// <param name="personId">Identificador da pessoa, opcional.</param>
    /// <param name="type">Tipo/finalidade do consentimento, opcional.</param>
    /// <param name="status">Situação: <c>"granted"</c> (ativos) ou <c>"revoked"</c> (revogados), opcional.</param>
    /// <param name="dataInicio">Início do período de concessão (UTC, inclusive), opcional.</param>
    /// <param name="dataFim">Fim do período de concessão (UTC, inclusive), opcional.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>200 com a lista de consentimentos.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<PersonConsentWithPersonResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(
        [FromQuery] Guid? personId,
        [FromQuery] ConsentType? type,
        [FromQuery] string? status,
        [FromQuery] DateTime? dataInicio,
        [FromQuery] DateTime? dataFim,
        CancellationToken cancellationToken)
    {
        bool? revoked = status switch
        {
            "granted" => false,
            "revoked" => true,
            _ => null,
        };

        var result = await _mediator.Send(
            new GetConsentsQuery(personId, type, revoked, dataInicio, dataFim), cancellationToken);

        return ToHttpResult(result);
    }
}
