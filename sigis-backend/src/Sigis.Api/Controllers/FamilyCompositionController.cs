using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sigis.Api.Contracts.FamilyComposition;
using Sigis.Application.UseCases.Family;
using Sigis.Domain.Abstractions;

namespace Sigis.Api.Controllers;

/// <summary>Controller de composição familiar de uma pessoa (<c>/api/pessoas/{pessoaId}/composicao-familiar</c>).</summary>
[Route("api/pessoas/{pessoaId:guid}/composicao-familiar")]
[Authorize(Policy = "RequireAuthenticated")]
[Produces("application/json")]
public sealed class FamilyCompositionController : ApiControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>Cria a controller de composição familiar.</summary>
    /// <param name="mediator">Despachante MediatR dos casos de uso.</param>
    public FamilyCompositionController(IMediator mediator) => _mediator = mediator;

    /// <summary>Consulta a composição familiar de uma pessoa.</summary>
    /// <param name="pessoaId">Identificador da pessoa.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>200 com a composição familiar, ou 404 quando a pessoa ou a composição não existirem.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(FamilyCompositionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] Guid pessoaId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetFamilyCompositionQuery(pessoaId), cancellationToken);
        return ToHttpResult(result);
    }

    /// <summary>Cria (se ainda não existir) ou atualiza a composição familiar de uma pessoa.</summary>
    /// <param name="pessoaId">Identificador da pessoa.</param>
    /// <param name="request">Dados da composição familiar.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>200 com a composição atualizada, 404 se a pessoa não existir, ou 400 em caso de regra violada.</returns>
    [HttpPut]
    [ProducesResponseType(typeof(FamilyCompositionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        [FromRoute] Guid pessoaId, [FromBody] UpdateFamilyCompositionRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateFamilyCompositionCommand(
            pessoaId, request.FatherName, request.FatherEducation, request.FatherOccupation, request.MotherName,
            request.MotherEducation, request.MotherOccupation, request.SiblingsCount, request.SiblingsAges,
            request.HouseholdMembersCount, request.ParentsMaritalStatus, request.FiliationType,
            request.PlannedPregnancy, request.PregnanciesCount, request.AbortionsCount,
            request.PregnancyHealthIssue, request.DeliveryType, request.MedicationDuringPregnancy);

        var result = await _mediator.Send(command, cancellationToken);
        return ToHttpResult(result);
    }
}
