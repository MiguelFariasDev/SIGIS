using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sigis.Application.UseCases.Indicators;

namespace Sigis.Api.Controllers;

/// <summary>Controller do painel de indicadores operacionais (<c>/api/indicadores</c>).</summary>
[Route("api/indicadores")]
[Produces("application/json")]
[Authorize]
public sealed class IndicatorsController : ApiControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>Cria a controller de indicadores.</summary>
    /// <param name="mediator">Despachante MediatR dos casos de uso.</param>
    public IndicatorsController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Retorna o painel de indicadores operacionais (fila, atendimentos e
    /// encaminhamentos), filtrado opcionalmente por unidade e período.
    /// </summary>
    /// <param name="unidadeId">Identificador da unidade de serviço, opcional.</param>
    /// <param name="dataInicio">Início do período (UTC, inclusive), opcional.</param>
    /// <param name="dataFim">Fim do período (UTC, inclusive), opcional.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>200 com o painel de indicadores.</returns>
    [HttpGet("painel")]
    [Authorize(Policy = "RequireAuthenticated")]
    [ProducesResponseType(typeof(IndicatorsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPainel(
        [FromQuery] Guid? unidadeId,
        [FromQuery] DateTime? dataInicio,
        [FromQuery] DateTime? dataFim,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetIndicatorsPanelQuery(unidadeId, dataInicio, dataFim), cancellationToken);

        return ToHttpResult(result);
    }

    /// <summary>Retorna a fila (aguardando/em atendimento) agrupada por unidade de serviço.</summary>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>200 com a fila por serviço.</returns>
    [HttpGet("fila-por-servico")]
    [Authorize(Policy = "RequireAuthenticated")]
    [ProducesResponseType(typeof(IReadOnlyList<FilaPorServicoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFilaPorServico(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetFilaPorServicoQuery(), cancellationToken);
        return ToHttpResult(result);
    }

    /// <summary>Retorna a quantidade de atendimentos por dia, no período informado.</summary>
    /// <param name="periodo">Período: <c>"HOJE"</c>, <c>"7D"</c> ou <c>"30D"</c>.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>200 com a série de atendimentos por dia.</returns>
    [HttpGet("atendimentos-por-dia")]
    [Authorize(Policy = "RequireAuthenticated")]
    [ProducesResponseType(typeof(IReadOnlyList<AtendimentosPorDiaResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAtendimentosPorDia([FromQuery] string? periodo, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAtendimentosPorDiaQuery(periodo), cancellationToken);
        return ToHttpResult(result);
    }

    /// <summary>Retorna os alertas recentes (duplicidades pendentes e entradas em busca ativa).</summary>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>200 com os alertas recentes.</returns>
    [HttpGet("alertas-recentes")]
    [Authorize(Policy = "RequireAuthenticated")]
    [ProducesResponseType(typeof(IReadOnlyList<AlertaRecenteResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAlertasRecentes(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAlertasRecentesQuery(), cancellationToken);
        return ToHttpResult(result);
    }
}
