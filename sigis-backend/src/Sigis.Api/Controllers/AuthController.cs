using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sigis.Api.Contracts.Auth;
using Sigis.Application.UseCases.Auth;
using Sigis.Domain.Abstractions;

namespace Sigis.Api.Controllers;

/// <summary>Controller de autenticação (<c>/api/auth</c>): login e verificação da sessão atual.</summary>
[Produces("application/json")]
public sealed class AuthController : ApiControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>Cria a controller de autenticação.</summary>
    /// <param name="mediator">Despachante MediatR dos casos de uso.</param>
    public AuthController(IMediator mediator) => _mediator = mediator;

    /// <summary>Autentica um profissional por e-mail e senha e retorna um token JWT.</summary>
    /// <param name="request">E-mail e senha do profissional.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>200 com o token de acesso, ou 401 quando as credenciais forem inválidas.</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new LoginCommand(request.Email, request.Password), cancellationToken);
        return ToHttpResult(result);
    }

    /// <summary>Retorna os dados do profissional autenticado a partir do token JWT da requisição.</summary>
    /// <returns>200 com os dados do profissional autenticado.</returns>
    [HttpGet("me")]
    [Authorize(Policy = "RequireAuthenticated")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Me()
    {
        var user = HttpContext.User;

        return Ok(new
        {
            professionalId = user.FindFirst("sub")?.Value,
            name = user.FindFirst("name")?.Value,
            email = user.FindFirst("email")?.Value,
            unitId = user.FindFirst("unit_id")?.Value,
            role = user.FindFirst("role")?.Value,
        });
    }
}
