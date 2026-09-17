using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Sigis.Domain.ValueObjects;

namespace Sigis.Infrastructure.Persistence.Converters;

/// <summary>
/// Converte <see cref="PhoneNumber"/> de e para a coluna de texto que o
/// armazena, já normalizado no formato E.164 pelo próprio value object.
/// </summary>
public sealed class PhoneNumberConverter : ValueConverter<PhoneNumber?, string>
{
    /// <summary>Cria o conversor de <see cref="PhoneNumber"/>.</summary>
    public PhoneNumberConverter()
        : base(
            vo => vo!.Value,
            str => PhoneNumber.Create(str).Value)
    {
    }
}
