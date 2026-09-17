namespace Sigis.Api.Contracts.Auth;

/// <summary>Requisição de login.</summary>
/// <param name="Email">E-mail de login.</param>
/// <param name="Password">Senha em texto claro.</param>
public sealed record LoginRequest(string Email, string Password);
