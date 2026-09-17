using Sigis.Domain.Abstractions;
using Sigis.Domain.Entities;

namespace Sigis.Domain.Interfaces;

/// <summary>
/// Serviço de emissão de tokens de acesso (JWT) para profissionais
/// autenticados.
/// </summary>
public interface ITokenService
{
    /// <summary>Gera um token de acesso para o profissional informado.</summary>
    /// <param name="professional">Profissional autenticado.</param>
    /// <returns>
    /// Um <see cref="Result{T}"/> de sucesso com o token gerado, ou de falha
    /// quando a configuração de emissão de tokens for inválida.
    /// </returns>
    Result<TokenResponse> GenerateToken(Professional professional);
}

/// <summary>Token de acesso emitido após um login bem-sucedido.</summary>
/// <param name="AccessToken">Token JWT assinado.</param>
/// <param name="ExpiresInSeconds">Tempo de vida do token, em segundos.</param>
public sealed record TokenResponse(string AccessToken, int ExpiresInSeconds);
