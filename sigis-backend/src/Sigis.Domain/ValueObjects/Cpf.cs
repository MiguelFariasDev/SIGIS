using CPFCNPJ;
using CPFCNPJ.Enum;
using FluentValidation;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Abstractions.Errors;

namespace Sigis.Domain.ValueObjects;

/// <summary>
/// Número de Cadastro de Pessoa Física (CPF) de uma pessoa ou de seu responsável.
/// </summary>
/// <remarks>
/// A validação do dígito verificador (módulo 11) usa o pacote NuGet
/// <c>CPFCNPJ</c>, uma biblioteca brasileira consolidada para essa
/// finalidade — não há motivo para reimplementar esse algoritmo na mão.
/// </remarks>
public sealed record Cpf
{
    private static readonly IMain CpfValidator = new Main();

    /// <summary>Número do CPF, com 11 dígitos numéricos, sem formatação.</summary>
    public string Value { get; }

    private Cpf(string value) => Value = value;

    /// <summary>
    /// Cria um <see cref="Cpf"/> válido a partir de uma string bruta (com ou
    /// sem máscara), validando o dígito verificador pelo algoritmo oficial.
    /// </summary>
    /// <param name="rawCpf">Número do CPF informado, com ou sem formatação.</param>
    /// <returns>
    /// Um <see cref="Result{T}"/> de sucesso com o número normalizado (apenas
    /// dígitos), ou de falha com <see cref="PersonErrors.CpfInvalido"/>.
    /// </returns>
    public static Result<Cpf> Create(string? rawCpf)
    {
        var digits = new string((rawCpf ?? string.Empty).Where(char.IsDigit).ToArray());

        var validationResult = new Validator().Validate(digits);
        if (!validationResult.IsValid)
            return Result<Cpf>.Failure(PersonErrors.CpfInvalido);

        return Result<Cpf>.Success(new Cpf(digits));
    }

    /// <summary>Formata o CPF no padrão "000.000.000-00".</summary>
    /// <returns>CPF formatado com máscara.</returns>
    public string Formatted() => CpfValidator.FormatCPFCNPJ(Value, TypeString.CPF);

    /// <summary>
    /// Validador interno (FluentValidation) do CPF: exatamente 11 dígitos e
    /// dígito verificador válido pela biblioteca <c>CPFCNPJ</c>.
    /// </summary>
    private sealed class Validator : AbstractValidator<string>
    {
        public Validator()
        {
            RuleFor(x => x).Must(v => v.Length == 11 && CpfValidator.IsValidCPFCNPJ(v));
        }
    }
}
