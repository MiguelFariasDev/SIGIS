using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sigis.Api.Contracts.ClinicalProfile;
using Sigis.Application.UseCases.ClinicalProfile;
using Sigis.Domain.Abstractions;

namespace Sigis.Api.Controllers;

/// <summary>Controller de perfil clínico NASF de uma pessoa (<c>/api/pessoas/{pessoaId}/perfil-clinico</c>).</summary>
[Route("api/pessoas/{pessoaId:guid}/perfil-clinico")]
[Authorize(Policy = "RequireAuthenticated")]
[Produces("application/json")]
public sealed class ClinicalProfileController : ApiControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>Cria a controller de perfil clínico.</summary>
    /// <param name="mediator">Despachante MediatR dos casos de uso.</param>
    public ClinicalProfileController(IMediator mediator) => _mediator = mediator;

    /// <summary>Consulta o perfil clínico NASF de uma pessoa.</summary>
    /// <param name="pessoaId">Identificador da pessoa.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>200 com o perfil clínico, ou 404 quando a pessoa ou o perfil não existirem.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PersonClinicalProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] Guid pessoaId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetClinicalProfileQuery(pessoaId), cancellationToken);
        return ToHttpResult(result);
    }

    /// <summary>Cria (se ainda não existir) ou atualiza o perfil clínico NASF de uma pessoa.</summary>
    /// <param name="pessoaId">Identificador da pessoa.</param>
    /// <param name="request">Dados do perfil clínico.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>
    /// 200 com o perfil atualizado, 404 se a pessoa não existir, 400 em caso
    /// de regra violada, ou 409 se o número do prontuário já existir na
    /// unidade APS informada.
    /// </returns>
    [HttpPut]
    [ProducesResponseType(typeof(PersonClinicalProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(
        [FromRoute] Guid pessoaId, [FromBody] UpdateClinicalProfileRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateClinicalProfileCommand(
            pessoaId, request.MedicalRecordNumber, request.ClinicalHypothesis, request.ApsReferenceUnitId);

        var result = await _mediator.Send(command, cancellationToken);
        return ToHttpResult(result);
    }
}
