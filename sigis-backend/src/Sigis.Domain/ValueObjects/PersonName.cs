using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using FluentValidation;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Abstractions.Errors;

namespace Sigis.Domain.ValueObjects;

/// <summary>
/// Nome completo de uma pessoa (ou de seu responsável), normalizado para
/// comparação e para busca fuzzy de duplicidade de cadastro.
/// </summary>
public sealed record PersonName
{
    private static readonly Regex ValidCharacters = new(@"^[\p{L}\s'\-]+$", RegexOptions.Compiled);
    private static readonly Regex CollapseWhitespace = new(@"\s+", RegexOptions.Compiled);

    /// <summary>Valor do nome completo, normalizado (trim e espaços múltiplos colapsados em um único espaço).</summary>
    public string Value { get; }

    /// <summary>
    /// Nome sem acentuação e em minúsculas, usado para comparação exata e
    /// como base para o cálculo de similaridade textual.
    /// </summary>
    public string Normalized { get; }

    private PersonName(string value, string normalized)
    {
        Value = value;
        Normalized = normalized;
    }

    /// <summary>
    /// Cria um <see cref="PersonName"/> válido a partir de uma string bruta,
    /// aplicando normalização (trim, colapso de espaços múltiplos) e
    /// validando as regras de nome completo.
    /// </summary>
    /// <param name="rawName">Nome completo informado pelo usuário.</param>
    /// <returns>
    /// Um <see cref="Result{T}"/> de sucesso com o nome normalizado, ou de
    /// falha com o primeiro erro de validação encontrado.
    /// </returns>
    public static Result<PersonName> Create(string? rawName)
    {
        var normalizedValue = NormalizeWhitespace(rawName);

        var validationResult = new Validator().Validate(normalizedValue);
        if (!validationResult.IsValid)
            return Result<PersonName>.Failure(MapError(validationResult.Errors[0].ErrorCode));

        var normalized = RemoveDiacritics(normalizedValue).ToLowerInvariant();
        return Result<PersonName>.Success(new PersonName(normalizedValue, normalized));
    }

    /// <summary>
    /// Calcula a similaridade textual entre este nome e outro, usando o
    /// coeficiente de Dice sobre trigramas de caracteres — uma aproximação em
    /// C# puro do algoritmo usado pela extensão <c>pg_trgm</c> do PostgreSQL,
    /// útil para pré-visualizar candidatos a duplicidade antes de consultar o
    /// banco de dados.
    /// </summary>
    /// <param name="other">Outro nome a ser comparado.</param>
    /// <returns>Valor entre 0 (nenhuma similaridade) e 1 (nomes idênticos).</returns>
    /// <exception cref="ArgumentNullException">Lançada quando <paramref name="other"/> é <see langword="null"/>.</exception>
    public double SimilarityTo(PersonName other)
    {
        ArgumentNullException.ThrowIfNull(other);

        var trigramsA = ExtractTrigrams(Normalized);
        var trigramsB = ExtractTrigrams(other.Normalized);

        if (trigramsA.Count == 0 || trigramsB.Count == 0)
            return trigramsA.Count == trigramsB.Count ? 1d : 0d;

        var intersection = trigramsA.Intersect(trigramsB).Count();
        return 2d * intersection / (trigramsA.Count + trigramsB.Count);
    }

    private static HashSet<string> ExtractTrigrams(string value)
    {
        var padded = $"  {value} ";
        var trigrams = new HashSet<string>();
        for (var i = 0; i < padded.Length - 2; i++)
            trigrams.Add(padded.Substring(i, 3));

        return trigrams;
    }

    private static string NormalizeWhitespace(string? rawName)
    {
        if (string.IsNullOrWhiteSpace(rawName))
            return string.Empty;

        return CollapseWhitespace.Replace(rawName.Trim(), " ");
    }

    private static string RemoveDiacritics(string value)
    {
        var decomposed = value.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder();

        foreach (var c in decomposed)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                builder.Append(c);
        }

        return builder.ToString().Normalize(NormalizationForm.FormC);
    }

    private static bool HaveAtLeastTwoWords(string value) => SplitWords(value).Length >= 2;

    private static bool AllWordsHaveMinLength(string value) => SplitWords(value).All(w => w.Length >= 2);

    private static string[] SplitWords(string value) => value.Split(' ', StringSplitOptions.RemoveEmptyEntries);

    private static bool HaveOnlyValidCharacters(string value) => ValidCharacters.IsMatch(value);

    private static Error MapError(string errorCode) => errorCode switch
    {
        nameof(PersonErrors.NomeVazio) => PersonErrors.NomeVazio,
        nameof(PersonErrors.NomeMuitoLongo) => PersonErrors.NomeMuitoLongo,
        nameof(PersonErrors.NomeMuitoCurto) => PersonErrors.NomeMuitoCurto,
        nameof(PersonErrors.NomeUmaPalavra) => PersonErrors.NomeUmaPalavra,
        nameof(PersonErrors.NomePalavraCurta) => PersonErrors.NomePalavraCurta,
        nameof(PersonErrors.NomeInvalido) => PersonErrors.NomeInvalido,
        _ => CommonErrors.InternalError
    };

    /// <summary>
    /// Validador interno (FluentValidation) das regras de nome completo:
    /// obrigatoriedade, tamanho mínimo/máximo, número mínimo de palavras,
    /// tamanho mínimo de cada palavra e caracteres válidos.
    /// </summary>
    private sealed class Validator : AbstractValidator<string>
    {
        public Validator()
        {
            ClassLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x)
                .NotEmpty().WithErrorCode(nameof(PersonErrors.NomeVazio));

            RuleFor(x => x)
                .MaximumLength(150).WithErrorCode(nameof(PersonErrors.NomeMuitoLongo));

            RuleFor(x => x)
                .Must(v => v.Length >= 5).WithErrorCode(nameof(PersonErrors.NomeMuitoCurto));

            RuleFor(x => x)
                .Must(HaveAtLeastTwoWords).WithErrorCode(nameof(PersonErrors.NomeUmaPalavra));

            RuleFor(x => x)
                .Must(AllWordsHaveMinLength).WithErrorCode(nameof(PersonErrors.NomePalavraCurta));

            RuleFor(x => x)
                .Must(HaveOnlyValidCharacters).WithErrorCode(nameof(PersonErrors.NomeInvalido));
        }
    }
}
