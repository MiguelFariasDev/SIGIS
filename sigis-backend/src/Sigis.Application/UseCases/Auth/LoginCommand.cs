using MediatR;
using Sigis.Domain.Abstractions;

namespace Sigis.Application.UseCases.Auth;

/// <summary>Comando de autenticação de um profissional por e-mail e senha.</summary>
/// <param name="Email">E-mail de login.</param>
/// <param name="Password">Senha em texto claro (nunca logada nem persistida).</param>
public sealed record LoginCommand(string Email, string Password) : IRequest<Result<LoginResponse>>;
