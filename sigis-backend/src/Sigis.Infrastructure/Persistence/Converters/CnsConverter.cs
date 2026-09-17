using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Sigis.Domain.ValueObjects;
using Sigis.Infrastructure.Services;

namespace Sigis.Infrastructure.Persistence.Converters;

/// <summary>
/// Converte <see cref="Cns"/> de e para a coluna de texto que o armazena,
/// criptografando o valor em repouso (LGPD, dado de saúde sensível) via
/// <see cref="IEncryptionService"/>.
/// </summary>
public sealed class CnsConverter : ValueConverter<Cns?, string>
{
    /// <summary>Cria o conversor de <see cref="Cns"/>.</summary>
    /// <param name="encryption">Serviço de criptografia usado para proteger o CNS em repouso.</param>
    public CnsConverter(IEncryptionService encryption)
        : base(
            vo => encryption.Encrypt(vo!.Value),
            str => Cns.Create(encryption.Decrypt(str)).Value)
    {
    }
}
