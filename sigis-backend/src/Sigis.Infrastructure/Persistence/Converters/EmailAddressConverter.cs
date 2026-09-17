using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Sigis.Domain.ValueObjects;

namespace Sigis.Infrastructure.Persistence.Converters;

/// <summary>
/// Converte <see cref="EmailAddress"/> de e para a coluna de texto que o
/// armazena, já normalizado em minúsculas pelo próprio value object.
/// </summary>
public sealed class EmailAddressConverter : ValueConverter<EmailAddress?, string>
{
    /// <summary>Cria o conversor de <see cref="EmailAddress"/>.</summary>
    public EmailAddressConverter()
        : base(
            vo => vo!.Value,
            str => EmailAddress.Create(str).Value)
    {
    }
}
