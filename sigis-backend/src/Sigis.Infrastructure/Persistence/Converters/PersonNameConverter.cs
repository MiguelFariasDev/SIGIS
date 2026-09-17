using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Sigis.Domain.ValueObjects;

namespace Sigis.Infrastructure.Persistence.Converters;

/// <summary>
/// Converte <see cref="PersonName"/> de e para a coluna de texto que o
/// armazena. A reconstrução a partir do banco sempre tem sucesso, pois o
/// valor já foi validado antes de ser persistido.
/// </summary>
public sealed class PersonNameConverter : ValueConverter<PersonName, string>
{
    /// <summary>Cria o conversor de <see cref="PersonName"/>.</summary>
    public PersonNameConverter()
        : base(
            vo => vo.Value,
            str => PersonName.Create(str).Value)
    {
    }
}
