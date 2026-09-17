using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Entities;
using Sigis.Domain.Interfaces;

namespace Sigis.Infrastructure.Services;

/// <summary>
/// Implementação de <see cref="ITokenService"/> que emite tokens JWT (HS256)
/// assinados com uma chave simétrica configurada em <c>Jwt:Secret</c> (ou na
/// variável de ambiente equivalente).
/// </summary>
public sealed class JwtTokenService : ITokenService
{
    private readonly byte[] _keyBytes;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expirationMinutes;

    /// <summary>
    /// Cria o serviço de emissão de tokens, lendo a configuração de
    /// <c>Jwt:Secret</c>, <c>Jwt:Issuer</c>, <c>Jwt:Audience</c> e
    /// <c>Jwt:ExpirationMinutes</c>.
    /// </summary>
    /// <param name="configuration">Configuração da aplicação.</param>
    /// <exception cref="InvalidOperationException">
    /// Lançada quando a chave JWT não está configurada ou tem menos de 32
    /// caracteres — indica falha de configuração da infraestrutura, não
    /// regra de negócio.
    /// </exception>
    public JwtTokenService(IConfiguration configuration)
    {
        var secret = configuration["Jwt:Secret"]
            ?? throw new InvalidOperationException(
                "Chave JWT não configurada. Defina 'Jwt:Secret' no appsettings ou a variável de ambiente equivalente.");

        if (secret.Length < 32)
            throw new InvalidOperationException("A chave JWT ('Jwt:Secret') deve ter pelo menos 32 caracteres.");

        _keyBytes = Encoding.UTF8.GetBytes(secret);
        _issuer = configuration["Jwt:Issuer"] ?? "sigis-api";
        _audience = configuration["Jwt:Audience"] ?? "sigis-clients";
        _expirationMinutes = configuration.GetValue<int?>("Jwt:ExpirationMinutes") ?? 480;
    }

    /// <inheritdoc />
    public Result<TokenResponse> GenerateToken(Professional professional)
    {
        var now = DateTime.UtcNow;
        var expires = now.AddMinutes(_expirationMinutes);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, professional.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("name", professional.Name.Value),
            new Claim("email", professional.Email.Value),
            new Claim("unit_id", professional.UnitId.ToString()),
            new Claim("role", professional.Role.ToString()),
        };

        var credentials = new SigningCredentials(new SymmetricSecurityKey(_keyBytes), SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            notBefore: now,
            expires: expires,
            signingCredentials: credentials);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        return Result<TokenResponse>.Success(new TokenResponse(accessToken, _expirationMinutes * 60));
    }
}
