using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sigis.Api.Contracts.Persons;
using Sigis.Application.UseCases.Persons;
using Sigis.Domain.Abstractions;

namespace Sigis.Api.Controllers;

/// <summary>
/// Controller do Cadastro Único de pessoas (<c>/api/pessoas</c>): cadastro
/// com verificação de duplicidade, busca, consulta detalhada, linha do
/// tempo consolidada, mesclagem de cadastros e solicitação de acesso
/// cross-unidade (LGPD).
/// </summary>
[Route("api/pessoas")]
[Authorize]
[Produces("application/json")]
public sealed class PersonsController : ApiControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>Cria a controller de pessoas.</summary>
    /// <param name="mediator">Despachante MediatR dos casos de uso.</param>
    public PersonsController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Cadastra uma nova pessoa. Duplicidade exata de CNS/CPF impede o
    /// cadastro (409). Duplicidade aproximada por nome+nascimento (RN02,
    /// camada 3) não impede o cadastro — ele é criado normalmente, um
    /// <c>DuplicateAlert</c> é aberto para cada candidato, e a resposta
    /// (200, não 201) traz os candidatos para revisão de um coordenador.
    /// </summary>
    /// <param name="request">Dados da pessoa a cadastrar.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>
    /// 201 Created quando não há suspeita de duplicidade; 200 OK com os
    /// candidatos quando há; 400 Bad Request em falha de validação; 409
    /// Conflict quando CNS ou CPF já existirem em outro cadastro.
    /// </returns>
    [HttpPost]
    [Authorize(Policy = "RequireAuthenticated")]
    [ProducesResponseType(typeof(CreatePersonResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(CreatePersonResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreatePersonRequest request, CancellationToken cancellationToken)
    {
        var command = new CreatePersonCommand(
            request.Name, request.BirthDate, request.Cns, request.Cpf, request.MotherName, request.Gender,
            request.RaceColor, request.Phone, request.Email, request.Street, request.Number,
            request.Neighborhood, request.City, request.State, request.ZipCode);

        var result = await _mediator.Send(command, cancellationToken);
        if (result.IsFailure)
            return ToHttpResult(result);

        return result.Value.Candidates.Count == 0 ? ToCreatedResult(result) : Ok(result.Value);
    }

    /// <summary>Busca pessoas por nome (parcial ou aproximado).</summary>
    /// <param name="termo">Termo de busca por nome.</param>
    /// <param name="limit">Quantidade máxima de resultados (padrão 20).</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>200 OK com a lista de pessoas encontradas, podendo ser vazia.</returns>
    [HttpGet("busca")]
    [Authorize(Policy = "RequireAuthenticated")]
    [ProducesResponseType(typeof(IReadOnlyList<PersonSummary>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search([FromQuery] string termo, [FromQuery] int limit, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new SearchPersonsQuery(termo, limit), cancellationToken);
        return ToHttpResult(result);
    }

    /// <summary>Consulta os dados completos de uma pessoa pelo identificador.</summary>
    /// <param name="id">Identificador da pessoa.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>200 OK com os dados da pessoa, ou 404 quando não existir.</returns>
    [HttpGet("{id:guid}")]
    [Authorize(Policy = "RequireAuthenticated")]
    [ProducesResponseType(typeof(PersonDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetPersonByIdQuery(id), cancellationToken);
        return ToHttpResult(result);
    }

    /// <summary>
    /// Consulta a linha do tempo consolidada de atendimentos e
    /// encaminhamentos de uma pessoa em toda a rede (RF08).
    /// </summary>
    /// <param name="id">Identificador da pessoa.</param>
    /// <param name="nivel">
    /// Nível de disclosure: "metadados" (datas/tipos/unidades apenas) ou
    /// "completo" (inclui motivo/queixa) — qualquer valor diferente de
    /// "metadados" é tratado como "completo".
    /// </param>
    /// <param name="justificativa">
    /// Justificativa do acesso — obrigatória para nível "completo" ou quando
    /// o acesso cruza a fronteira de secretarias (LGPD, art. 11, II).
    /// </param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>200 OK com a linha do tempo, 403 quando a justificativa exigida não for informada, ou 404 quando a pessoa não existir.</returns>
    [HttpGet("{id:guid}/linha-do-tempo")]
    [Authorize(Policy = "RequireAuthenticated")]
    [ProducesResponseType(typeof(TimelineResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTimeline(
        Guid id, [FromQuery] string? nivel, [FromQuery] string? justificativa, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetPersonTimelineQuery(id, nivel, justificativa), cancellationToken);
        return ToHttpResult(result);
    }

    /// <summary>
    /// Mescla dois cadastros de pessoa reconhecidos como duplicados. Apenas
    /// coordenadores podem confirmar mesclagens (RN02) — cadastros nunca são
    /// fundidos automaticamente pelo sistema.
    /// </summary>
    /// <param name="request">Cadastros de origem e destino, e o alerta de duplicidade relacionado, quando houver.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>200 OK com o resultado da mesclagem, 400 quando origem e destino forem a mesma pessoa, ou 404 quando algum cadastro não existir.</returns>
    [HttpPost("mesclar")]
    [Authorize(Policy = "RequireCoordinator")]
    [ProducesResponseType(typeof(MergeResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Merge([FromBody] MergePersonsRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new MergePersonsCommand(request.SourcePersonId, request.TargetPersonId, request.DuplicateAlertId),
            cancellationToken);

        return ToHttpResult(result);
    }

    /// <summary>
    /// Solicita acesso completo a um cadastro fora da unidade do
    /// profissional (LGPD, art. 11, II) — registra o acesso em auditoria com
    /// a justificativa informada.
    /// </summary>
    /// <param name="id">Identificador da pessoa.</param>
    /// <param name="request">Justificativa do acesso.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>200 OK com os dados completos da pessoa, 400 quando a justificativa não for informada, ou 404 quando a pessoa não existir.</returns>
    [HttpPost("{id:guid}/solicitar-acesso")]
    [Authorize(Policy = "RequireAuthenticated")]
    [ProducesResponseType(typeof(PersonDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RequestAccess(
        Guid id, [FromBody] SolicitarAcessoRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new RequestPersonAccessCommand(id, request.Justificativa), cancellationToken);
        return ToHttpResult(result);
    }
}
