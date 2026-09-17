using Sigis.Domain.Interfaces;

namespace Sigis.Infrastructure.Services;

/// <summary>
/// Implementação de <see cref="IPasswordHasher"/> baseada em BCrypt
/// (<c>BCrypt.Net-Next</c>), com fator de custo 12 — nunca armazena nem
/// compara senhas em texto claro.
/// </summary>
public sealed class BcryptPasswordHasher : IPasswordHasher
{
    private const int WorkFactor = 12;

    /// <inheritdoc />
    public string Hash(string plainPassword)
        => BCrypt.Net.BCrypt.HashPassword(plainPassword, workFactor: WorkFactor);

    /// <inheritdoc />
    public bool Verify(string plainPassword, string passwordHash)
        => BCrypt.Net.BCrypt.Verify(plainPassword, passwordHash);
}
