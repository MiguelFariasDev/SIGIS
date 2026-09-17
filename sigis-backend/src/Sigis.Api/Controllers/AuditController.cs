using System.Text;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sigis.Api.Extensions;
using Sigis.Application.UseCases.Audit;

namespace Sigis.Api.Controllers;

/// <summary>Controller de auditoria de acesso (<c>/api/auditoria</c>) — restrita ao papel Auditor (RF13).</summary>
[Route("api/auditoria")]
[Produces("application/json")]
[Authorize(Policy = "RequireAuditor")]
public sealed class AuditController : ApiControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>Cria a controller de auditoria.</summary>
    /// <param name="mediator">Despachante MediatR dos casos de uso.</param>
    public AuditController(IMediator mediator) => _mediator = mediator;

    /// <summary>Lista os registros de auditoria de acesso, filtrados opcionalmente por pessoa, profissional e período.</summary>
    /// <param name="pessoaId">Identificador da pessoa, opcional.</param>
    /// <param name="profissionalId">Identificador do profissional, opcional.</param>
    /// <param name="dataInicio">Início do período (UTC, inclusive), opcional.</param>
    /// <param name="dataFim">Fim do período (UTC, inclusive), opcional.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>200 com a lista de registros de auditoria.</returns>
    [HttpGet("acessos")]
    [ProducesResponseType(typeof(IReadOnlyList<AccessLogResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAcessos(
        [FromQuery] Guid? pessoaId,
        [FromQuery] Guid? profissionalId,
        [FromQuery] DateTime? dataInicio,
        [FromQuery] DateTime? dataFim,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetAccessLogsQuery(pessoaId, profissionalId, dataInicio, dataFim), cancellationToken);

        return ToHttpResult(result);
    }

    /// <summary>Lista os registros de auditoria de acesso que cruzaram a fronteira de unidades/secretarias (RNF02).</summary>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>200 com a lista de acessos entre unidades.</returns>
    [HttpGet("acessos-cross")]
    [ProducesResponseType(typeof(IReadOnlyList<CrossAccessResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAcessosCross(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetCrossUnitAccessLogsQuery(), cancellationToken);
        return ToHttpResult(result);
    }

    /// <summary>Exporta os registros de auditoria de acesso em CSV, com os mesmos filtros de <see cref="GetAcessos"/>.</summary>
    /// <param name="pessoaId">Identificador da pessoa, opcional.</param>
    /// <param name="profissionalId">Identificador do profissional, opcional.</param>
    /// <param name="dataInicio">Início do período (UTC, inclusive), opcional.</param>
    /// <param name="dataFim">Fim do período (UTC, inclusive), opcional.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>200 com o arquivo CSV.</returns>
    [HttpGet("export-csv")]
    [Produces("text/csv")]
    public async Task<IActionResult> ExportCsv(
        [FromQuery] Guid? pessoaId,
        [FromQuery] Guid? profissionalId,
        [FromQuery] DateTime? dataInicio,
        [FromQuery] DateTime? dataFim,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new ExportAccessLogsCsvQuery(pessoaId, profissionalId, dataInicio, dataFim), cancellationToken);

        if (result.IsFailure)
            return result.Error.ToErrorResult();

        return File(Encoding.UTF8.GetBytes(result.Value), "text/csv", "auditoria-sigis.csv");
    }
}
