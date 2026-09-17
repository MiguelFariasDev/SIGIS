namespace Sigis.Infrastructure.Services;

/// <summary>
/// Serviço de criptografia simétrica usado para proteger dados sensíveis em
/// repouso (CNS e CPF), conforme exigido pela LGPD (art. 46) — ver
/// <c>Sigis.Infrastructure.Persistence.Converters.CnsConverter</c> e
/// <c>CpfConverter</c>.
/// </summary>
public interface IEncryptionService
{
    /// <summary>Criptografa um texto em claro.</summary>
    /// <param name="plainText">Texto original a ser criptografado.</param>
    /// <returns>Texto cifrado, codificado em Base64, pronto para persistência.</returns>
    string Encrypt(string plainText);

    /// <summary>Descriptografa um texto previamente cifrado por <see cref="Encrypt"/>.</summary>
    /// <param name="cipherText">Texto cifrado, codificado em Base64.</param>
    /// <returns>Texto original em claro.</returns>
    string Decrypt(string cipherText);
}
