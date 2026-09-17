using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sigis.Application.UseCases.Units;

namespace Sigis.Api.Controllers;

/// <summary>Controller de unidades de serviço (<c>/api/unidades</c>): listagem para seletores da UI (ex.: destino de encaminhamento).</summary>
[Route("api/unidades")]
[Authorize]
[Produces("application/json")]
public sealed class UnitsController : ApiControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>Cria a controller de unidades.</summary>
    /// <param name="mediator">Despachante MediatR dos casos de uso.</param>
    public UnitsController(IMediator mediator) => _mediator = mediator;

    /// <summary>Lista todas as unidades de serviço da rede.</summary>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>200 OK com a lista de unidades.</returns>
    [HttpGet]
    [Authorize(Policy = "RequireAuthenticated")]
    [ProducesResponseType(typeof(IReadOnlyList<ServiceUnitResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllServiceUnitsQuery(), cancellationToken);
        return ToHttpResult(result);
    }
}
