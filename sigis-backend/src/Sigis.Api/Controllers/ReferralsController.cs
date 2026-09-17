using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sigis.Api.Contracts.Referrals;
using Sigis.Application.UseCases.Referrals;
using Sigis.Domain.Abstractions;

namespace Sigis.Api.Controllers;

/// <summary>Controller de Encaminhamentos entre unidades de serviço (<c>/api/encaminhamentos</c>).</summary>
[Authorize(Policy = "RequireAuthenticated")]
[Produces("application/json")]
[Route("api/encaminhamentos")]
public sealed class ReferralsController : ApiControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>Cria a controller de encaminhamentos.</summary>
    /// <param name="mediator">Despachante MediatR dos casos de uso.</param>
    public ReferralsController(IMediator mediator) => _mediator = mediator;

    /// <summary>Encaminha uma pessoa de uma unidade de origem para uma unidade de destino.</summary>
    /// <param name="request">Dados do encaminhamento.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>201 com o encaminhamento criado, ou 400/403/404 conforme a falha.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ReferralResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Refer([FromBody] ReferPersonRequest request, CancellationToken cancellationToken)
    {
        var command = new ReferPersonCommand(
            request.PersonId, request.OriginUnitId, request.DestinationUnitId, request.Reason, request.Priority);

        var result = await _mediator.Send(command, cancellationToken);
        return ToCreatedResult(result);
    }

    /// <summary>Lista os encaminhamentos recebidos pela unidade do profissional autenticado.</summary>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>200 com a lista de encaminhamentos recebidos.</returns>
    [HttpGet("recebidos")]
    [ProducesResponseType(typeof(List<ReferralResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetReceived(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetReceivedReferralsQuery(), cancellationToken);
        return ToHttpResult(result);
    }

    /// <summary>Lista todos os encaminhamentos enviados pela unidade do profissional autenticado.</summary>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>200 com a lista de encaminhamentos enviados.</returns>
    [HttpGet("enviados")]
    [ProducesResponseType(typeof(List<ReferralResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetSent(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetSentReferralsQuery(), cancellationToken);
        return ToHttpResult(result);
    }

    /// <summary>Lista o histórico de encaminhamentos de uma pessoa em todas as unidades da rede.</summary>
    /// <param name="pessoaId">Identificador da pessoa.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>200 com a lista de encaminhamentos da pessoa.</returns>
    [HttpGet("pessoa/{pessoaId:guid}")]
    [ProducesResponseType(typeof(List<ReferralResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByPerson(Guid pessoaId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetReferralsByPersonQuery(pessoaId), cancellationToken);
        return ToHttpResult(result);
    }

    /// <summary>Aceita um encaminhamento pendente.</summary>
    /// <param name="id">Identificador do encaminhamento.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>200 com o encaminhamento atualizado, ou 409 quando não estiver pendente.</returns>
    [HttpPost("{id:guid}/aceitar")]
    [ProducesResponseType(typeof(ReferralResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Accept(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new AcceptReferralCommand(id), cancellationToken);
        return ToHttpResult(result);
    }

    /// <summary>Recusa um encaminhamento pendente.</summary>
    /// <param name="id">Identificador do encaminhamento.</param>
    /// <param name="request">Motivo da recusa.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>200 com o encaminhamento atualizado, ou 409 quando não estiver pendente.</returns>
    [HttpPost("{id:guid}/recusar")]
    [ProducesResponseType(typeof(ReferralResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Refuse(
        Guid id, [FromBody] RefuseReferralRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new RefuseReferralCommand(id, request.Motivo), cancellationToken);
        return ToHttpResult(result);
    }

    /// <summary>Consulta o rastreio (situação atual e unidades envolvidas) de um encaminhamento.</summary>
    /// <param name="id">Identificador do encaminhamento.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>200 com os dados de rastreio, ou 404 quando o encaminhamento não existir.</returns>
    [HttpGet("{id:guid}/rastreio")]
    [ProducesResponseType(typeof(ReferralResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTracking(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetReferralTrackingQuery(id), cancellationToken);
        return ToHttpResult(result);
    }
}
