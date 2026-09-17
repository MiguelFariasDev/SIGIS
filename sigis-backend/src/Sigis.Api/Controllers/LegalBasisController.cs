using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sigis.Application.UseCases.LegalBasis;
using Sigis.Domain.Enums;

namespace Sigis.Api.Controllers;

/// <summary>
/// Controller de referência de bases legais LGPD por secretaria
/// (<c>/api/legal-basis</c>) — conteúdo estático, sem persistência.
/// </summary>
[Route("api/legal-basis")]
[Produces("application/json")]
[Authorize]
public sealed class LegalBasisController : ApiControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>Cria a controller de bases legais.</summary>
    /// <param name="mediator">Despachante MediatR dos casos de uso.</param>
    public LegalBasisController(IMediator mediator) => _mediator = mediator;

    /// <summary>Lista as bases legais (LGPD) aplicáveis, filtradas opcionalmente por secretaria.</summary>
    /// <param name="secretariat">Secretaria a filtrar, opcional.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>200 com a lista de bases legais.</returns>
    [HttpGet]
    [Authorize(Policy = "RequireAuthenticated")]
    [ProducesResponseType(typeof(IReadOnlyList<LegalBasisResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(
        [FromQuery] ResponsibleSecretariat? secretariat, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetLegalBasisQuery(secretariat), cancellationToken);
        return ToHttpResult(result);
    }
}
