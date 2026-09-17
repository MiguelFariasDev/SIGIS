using System.Text.RegularExpressions;
using FluentValidation;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Abstractions.Errors;

namespace Sigis.Domain.ValueObjects;

/// <summary>
/// Número do Cartão Nacional de Saúde (CNS) de uma pessoa ou de seu responsável.
/// </summary>
/// <remarks>
/// Diferente de CPF/CNPJ (cobertos pelo pacote NuGet <c>CPFCNPJ</c>), não
/// existe biblioteca .NET consolidada exclusiva para validação de CNS. Por
/// isso, o algoritmo oficial de validação do dígito verificador — publicado
/// pelo DATASUS e amplamente replicado na comunidade de desenvolvimento
/// brasileira — é implementado diretamente aqui, isolado e testável, em vez
/// de depender de uma dependência externa de baixa confiabilidade.
/// </remarks>
public sealed record Cns
{
    private static readonly Regex OnlyDigits = new(@"^\d{15}$", RegexOptions.Compiled);

    /// <summary>Número do CNS, com exatamente 15 dígitos numéricos, sem formatação.</summary>
    public string Value { get; }

    private Cns(string value) => Value = value;

    /// <summary>
    /// Cria um <see cref="Cns"/> válido a partir de uma string bruta,
    /// validando formato (15 dígitos) e dígito verificador pelo algoritmo
    /// oficial do CNS.
    /// </summary>
    /// <param name="rawCns">Número do CNS informado, com ou sem formatação.</param>
    /// <returns>
    /// Um <see cref="Result{T}"/> de sucesso com o número normalizado (apenas
    /// dígitos), ou de falha com <see cref="PersonErrors.CnsInvalido"/>.
    /// </returns>
    public static Result<Cns> Create(string? rawCns)
    {
        var digits = ExtractDigits(rawCns);

        var validationResult = new Validator().Validate(digits);
        if (!validationResult.IsValid)
            return Result<Cns>.Failure(PersonErrors.CnsInvalido);

        return Result<Cns>.Success(new Cns(digits));
    }

    private static string ExtractDigits(string? rawCns) =>
        new((rawCns ?? string.Empty).Where(char.IsDigit).ToArray());

    /// <summary>
    /// Valida o dígito verificador de um CNS de 15 dígitos pelo algoritmo
    /// oficial publicado pelo DATASUS, que difere entre CNS definitivo
    /// (iniciado em 1 ou 2, derivado do PIS/PASEP) e CNS provisório
    /// (iniciado em 7, 8 ou 9).
    /// </summary>
    /// <param name="digits">Número do CNS com exatamente 15 dígitos numéricos.</param>
    /// <returns><see langword="true"/> quando o dígito verificador é válido.</returns>
    private static bool IsCheckDigitValid(string digits)
    {
        if (digits.Length != 15)
            return false;

        return digits[0] switch
        {
            '1' or '2' => IsDefinitivoValid(digits),
            '7' or '8' or '9' => IsProvisorioValid(digits),
            _ => false
        };
    }

    private static bool IsDefinitivoValid(string digits)
    {
        var pis = digits[..11];
        var soma = 0;
        for (var i = 0; i < 11; i++)
            soma += (pis[i] - '0') * (15 - i);

        var resto = soma % 11;
        var dv = 11 - resto;

        string resultado;
        if (dv == 11)
        {
            resultado = $"{pis}0000";
        }
        else if (dv == 10)
        {
            soma += 2;
            resto = soma % 11;
            dv = 11 - resto;
            resultado = $"{pis}001{dv}";
        }
        else
        {
            resultado = $"{pis}000{dv}";
        }

        return resultado == digits;
    }

    private static bool IsProvisorioValid(string digits)
    {
        var soma = 0;
        for (var i = 0; i < 15; i++)
            soma += (digits[i] - '0') * (15 - i);

        return soma % 11 == 0;
    }

    /// <summary>
    /// Validador interno (FluentValidation) do CNS: formato de 15 dígitos
    /// numéricos e dígito verificador válido.
    /// </summary>
    private sealed class Validator : AbstractValidator<string>
    {
        public Validator()
        {
            RuleFor(x => x).Must(v => OnlyDigits.IsMatch(v) && IsCheckDigitValid(v));
        }
    }
}
