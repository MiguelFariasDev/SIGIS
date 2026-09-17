using FluentValidation;
using PhoneNumbers;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Abstractions.Errors;

namespace Sigis.Domain.ValueObjects;

/// <summary>
/// Número de telefone de contato de uma pessoa, validado para a região
/// brasileira (BR).
/// </summary>
/// <remarks>
/// A validação e a formatação usam o pacote <c>libphonenumber-csharp</c>,
/// port oficial em C# da biblioteca <c>libphonenumber</c> do Google — evita
/// reimplementar regras de DDD, comprimento e prefixos de operadora na mão.
/// </remarks>
public sealed record PhoneNumber
{
    private static readonly PhoneNumberUtil PhoneUtil = PhoneNumberUtil.GetInstance();

    /// <summary>Número de telefone normalizado no formato E.164 (ex.: "+5588999998888").</summary>
    public string Value { get; }

    private PhoneNumber(string value) => Value = value;

    /// <summary>
    /// Cria um <see cref="PhoneNumber"/> válido a partir de uma string bruta,
    /// interpretando-a como número de telefone da região brasileira (BR).
    /// </summary>
    /// <param name="rawPhoneNumber">Número de telefone informado, com ou sem máscara/DDI.</param>
    /// <returns>
    /// Um <see cref="Result{T}"/> de sucesso com o número normalizado em
    /// E.164, ou de falha com <see cref="PersonErrors.TelefoneInvalido"/>.
    /// </returns>
    public static Result<PhoneNumber> Create(string? rawPhoneNumber)
    {
        var input = rawPhoneNumber ?? string.Empty;

        var validationResult = new Validator().Validate(input);
        if (!validationResult.IsValid)
            return Result<PhoneNumber>.Failure(PersonErrors.TelefoneInvalido);

        try
        {
            var parsed = PhoneUtil.Parse(input, "BR");
            if (!PhoneUtil.IsValidNumber(parsed))
                return Result<PhoneNumber>.Failure(PersonErrors.TelefoneInvalido);

            var e164 = PhoneUtil.Format(parsed, PhoneNumberFormat.E164);
            return Result<PhoneNumber>.Success(new PhoneNumber(e164));
        }
        catch (NumberParseException)
        {
            return Result<PhoneNumber>.Failure(PersonErrors.TelefoneInvalido);
        }
    }

    /// <summary>Formata o telefone no padrão nacional brasileiro (ex.: "(88) 99999-8888").</summary>
    /// <returns>Telefone formatado para exibição.</returns>
    public string Formatted()
    {
        var parsed = PhoneUtil.Parse(Value, "BR");
        return PhoneUtil.Format(parsed, PhoneNumberFormat.NATIONAL);
    }

    /// <summary>
    /// Validador interno (FluentValidation) de pré-condição: garante que uma
    /// entrada não vazia seja passada ao parser do <c>libphonenumber</c>, que
    /// lança exceção para entrada vazia em vez de reportar número inválido.
    /// </summary>
    private sealed class Validator : AbstractValidator<string>
    {
        public Validator()
        {
            RuleFor(x => x).NotEmpty();
        }
    }
}
