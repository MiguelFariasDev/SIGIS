namespace Sigis.Domain.Interfaces;

/// <summary>
/// Serviço de hashing de senhas, usado para nunca armazenar ou comparar
/// senhas em texto claro.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>Gera o hash de uma senha em claro.</summary>
    /// <param name="plainPassword">Senha em texto claro.</param>
    /// <returns>Hash da senha, pronto para persistência.</returns>
    string Hash(string plainPassword);

    /// <summary>Verifica se uma senha em claro corresponde a um hash previamente gerado.</summary>
    /// <param name="plainPassword">Senha em texto claro informada pelo usuário.</param>
    /// <param name="passwordHash">Hash armazenado para comparação.</param>
    /// <returns><see langword="true"/> quando a senha corresponde ao hash.</returns>
    bool Verify(string plainPassword, string passwordHash);
}
