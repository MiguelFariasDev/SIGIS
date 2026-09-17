using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Sigis.Domain.ValueObjects;
using Sigis.Infrastructure.Services;

namespace Sigis.Infrastructure.Persistence.Converters;

/// <summary>
/// Converte <see cref="Cpf"/> de e para a coluna de texto que o armazena,
/// criptografando o valor em repouso (LGPD, dado pessoal sensível) via
/// <see cref="IEncryptionService"/>.
/// </summary>
public sealed class CpfConverter : ValueConverter<Cpf?, string>
{
    /// <summary>Cria o conversor de <see cref="Cpf"/>.</summary>
    /// <param name="encryption">Serviço de criptografia usado para proteger o CPF em repouso.</param>
    public CpfConverter(IEncryptionService encryption)
        : base(
            vo => encryption.Encrypt(vo!.Value),
            str => Cpf.Create(encryption.Decrypt(str)).Value)
    {
    }
}
