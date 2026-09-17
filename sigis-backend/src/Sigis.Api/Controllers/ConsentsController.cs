using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sigis.Api.Contracts.Consents;
using Sigis.Application.UseCases.Consents;
using Sigis.Domain.Abstractions;

namespace Sigis.Api.Controllers;

/// <summary>Controller de consentimentos LGPD de uma pessoa (<c>/api/pessoas/{pessoaId}/consentimentos</c>).</summary>
[Route("api/pessoas/{pessoaId:guid}/consentimentos")]
[Authorize(Policy = "RequireAuthenticated")]
[Produces("application/json")]
public sealed class ConsentsController : ApiControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>Cria a controller de consentimentos.</summary>
    /// <param name="mediator">Despachante MediatR dos casos de uso.</param>
    public ConsentsController(IMediator mediator) => _mediator = mediator;

    /// <summary>Lista todos os consentimentos (ativos e revogados) de uma pessoa.</summary>
    /// <param name="pessoaId">Identificador da pessoa.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>200 com a lista de consentimentos, ou 404 quando a pessoa não existir.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<PersonConsentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] Guid pessoaId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetPersonConsentsQuery(pessoaId), cancellationToken);
        return ToHttpResult(result);
    }

    /// <summary>Concede um novo consentimento LGPD para a pessoa.</summary>
    /// <param name="pessoaId">Identificador da pessoa.</param>
    /// <param name="request">Dados do consentimento.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>201 com o consentimento criado, 404 se a pessoa não existir, ou 409 se já houver consentimento ativo do mesmo tipo.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(PersonConsentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Grant(
        [FromRoute] Guid pessoaId, [FromBody] GrantConsentRequest request, CancellationToken cancellationToken)
    {
        var command = new GrantConsentCommand(pessoaId, request.Type, request.Version, request.Evidence, request.GrantedByGuardianId);
        var result = await _mediator.Send(command, cancellationToken);
        return ToCreatedResult(result);
    }

    /// <summary>Revoga um consentimento LGPD já concedido a uma pessoa.</summary>
    /// <param name="pessoaId">Identificador da pessoa.</param>
    /// <param name="id">Identificador do consentimento.</param>
    /// <param name="request">Justificativa da revogação.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>204 quando revogado com sucesso, 404 se não existir, ou 409 se já estiver revogado.</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Revoke(
        [FromRoute] Guid pessoaId, [FromRoute] Guid id, [FromBody] RevokeConsentRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new RevokeConsentCommand(pessoaId, id, request.Justification), cancellationToken);
        return ToHttpResult(result);
    }
}
