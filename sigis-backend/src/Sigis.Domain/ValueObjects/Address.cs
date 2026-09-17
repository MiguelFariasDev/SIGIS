using FluentValidation;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Abstractions.Errors;

namespace Sigis.Domain.ValueObjects;

/// <summary>Endereço residencial de uma pessoa.</summary>
public sealed record Address
{
    /// <summary>Nome do logradouro.</summary>
    public string Street { get; }

    /// <summary>Número do imóvel.</summary>
    public string Number { get; }

    /// <summary>Bairro.</summary>
    public string Neighborhood { get; }

    /// <summary>Cidade.</summary>
    public string City { get; }

    /// <summary>Unidade federativa, com sigla de 2 letras (ex.: "CE").</summary>
    public string State { get; }

    /// <summary>CEP, quando informado.</summary>
    public string? ZipCode { get; }

    private Address(string street, string number, string neighborhood, string city, string state, string? zipCode)
    {
        Street = street;
        Number = number;
        Neighborhood = neighborhood;
        City = city;
        State = state;
        ZipCode = zipCode;
    }

    /// <summary>
    /// Cria um <see cref="Address"/> válido a partir dos campos informados.
    /// </summary>
    /// <param name="street">Nome do logradouro.</param>
    /// <param name="number">Número do imóvel.</param>
    /// <param name="neighborhood">Bairro.</param>
    /// <param name="city">Cidade.</param>
    /// <param name="state">Unidade federativa (sigla de 2 letras).</param>
    /// <param name="zipCode">CEP, opcional.</param>
    /// <returns>
    /// Um <see cref="Result{T}"/> de sucesso com o endereço validado, ou de
    /// falha com <see cref="PersonErrors.EnderecoInvalido"/>.
    /// </returns>
    public static Result<Address> Create(
        string? street,
        string? number,
        string? neighborhood,
        string? city,
        string? state,
        string? zipCode = null)
    {
        var input = new AddressInput(
            (street ?? string.Empty).Trim(),
            (number ?? string.Empty).Trim(),
            (neighborhood ?? string.Empty).Trim(),
            (city ?? string.Empty).Trim(),
            (state ?? string.Empty).Trim().ToUpperInvariant(),
            string.IsNullOrWhiteSpace(zipCode) ? null : zipCode.Trim());

        var validationResult = new Validator().Validate(input);
        if (!validationResult.IsValid)
            return Result<Address>.Failure(PersonErrors.EnderecoInvalido);

        return Result<Address>.Success(
            new Address(input.Street, input.Number, input.Neighborhood, input.City, input.State, input.ZipCode));
    }

    /// <summary>Retorna o endereço formatado como uma única linha legível.</summary>
    /// <returns>Endereço completo formatado (ex.: "Rua Tal, 123, Bairro X, Crateús/CE").</returns>
    public string Full()
    {
        var zip = string.IsNullOrWhiteSpace(ZipCode) ? string.Empty : $" - CEP {ZipCode}";
        return $"{Street}, {Number}, {Neighborhood}, {City}/{State}{zip}";
    }

    private sealed record AddressInput(
        string Street, string Number, string Neighborhood, string City, string State, string? ZipCode);

    /// <summary>
    /// Validador interno (FluentValidation) do endereço: logradouro, número,
    /// bairro e cidade obrigatórios, e UF com exatamente 2 letras.
    /// </summary>
    private sealed class Validator : AbstractValidator<AddressInput>
    {
        public Validator()
        {
            RuleFor(x => x.Street).NotEmpty();
            RuleFor(x => x.Number).NotEmpty();
            RuleFor(x => x.Neighborhood).NotEmpty();
            RuleFor(x => x.City).NotEmpty();
            RuleFor(x => x.State).NotEmpty().Length(2);
        }
    }
}
