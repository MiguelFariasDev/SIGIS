using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sigis.Api.Contracts.Attendances;
using Sigis.Application.UseCases.Attendances;
using Sigis.Domain.Abstractions;

namespace Sigis.Api.Controllers;

/// <summary>Controller de Atendimentos e Histórico (<c>/api/atendimentos</c>).</summary>
[Authorize(Policy = "RequireAuthenticated")]
[Produces("application/json")]
[Route("api/atendimentos")]
public sealed class AttendancesController : ApiControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>Cria a controller de atendimentos.</summary>
    /// <param name="mediator">Despachante MediatR dos casos de uso.</param>
    public AttendancesController(IMediator mediator) => _mediator = mediator;

    /// <summary>Registra um novo atendimento.</summary>
    /// <param name="request">Dados do atendimento a registrar.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>201 com o atendimento criado, ou 400/404 quando os dados forem inválidos.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(AttendanceResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterAttendanceRequest request, CancellationToken cancellationToken)
    {
        var command = new RegisterAttendanceCommand(
            request.PersonId, request.UnitId, request.ProfessionalId, request.DateTime, request.SessionType,
            request.FormData, request.TriagedByProfessionalId, request.MainComplaint);

        var result = await _mediator.Send(command, cancellationToken);
        return ToCreatedResult(result);
    }

    /// <summary>Consulta um atendimento pelo identificador.</summary>
    /// <param name="id">Identificador do atendimento.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>200 com o atendimento, ou 404 quando não existir.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AttendanceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAttendanceByIdQuery(id), cancellationToken);
        return ToHttpResult(result);
    }

    /// <summary>Lista o histórico de atendimentos de uma pessoa em todas as unidades da rede.</summary>
    /// <param name="pessoaId">Identificador da pessoa.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>200 com a lista de atendimentos da pessoa.</returns>
    [HttpGet("pessoa/{pessoaId:guid}")]
    [ProducesResponseType(typeof(List<AttendanceResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByPerson(Guid pessoaId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAttendancesByPersonQuery(pessoaId), cancellationToken);
        return ToHttpResult(result);
    }

    /// <summary>Registra o comparecimento ou a falta de uma pessoa a um atendimento agendado.</summary>
    /// <param name="id">Identificador do atendimento.</param>
    /// <param name="request">Situação de comparecimento e queixa principal opcional.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>200 com o atendimento atualizado, ou 409 quando o comparecimento já tiver sido registrado.</returns>
    [HttpPatch("{id:guid}/comparecimento")]
    [ProducesResponseType(typeof(AttendanceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegisterComparecimento(
        Guid id, [FromBody] RegisterAttendanceComparecimentoRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new RegisterAttendanceComparecimentoCommand(id, request.Comparecimento, request.MainComplaint),
            cancellationToken);
        return ToHttpResult(result);
    }
}
