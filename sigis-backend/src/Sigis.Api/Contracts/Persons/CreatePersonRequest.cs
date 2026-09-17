namespace Sigis.Api.Contracts.Persons;

/// <summary>Requisição de cadastro de uma nova pessoa.</summary>
/// <param name="Name">Nome completo.</param>
/// <param name="BirthDate">Data de nascimento.</param>
/// <param name="Cns">Número do CNS, opcional.</param>
/// <param name="Cpf">Número do CPF, opcional.</param>
/// <param name="MotherName">Nome completo da mãe, opcional.</param>
/// <param name="Gender">Sexo, opcional.</param>
/// <param name="RaceColor">Cor/raça, opcional.</param>
/// <param name="Phone">Telefone de contato, opcional.</param>
/// <param name="Email">E-mail de contato, opcional.</param>
/// <param name="Street">Logradouro do endereço, opcional (exige os demais campos de endereço quando informado).</param>
/// <param name="Number">Número do imóvel, opcional.</param>
/// <param name="Neighborhood">Bairro, opcional.</param>
/// <param name="City">Cidade, opcional.</param>
/// <param name="State">Unidade federativa (sigla de 2 letras), opcional.</param>
/// <param name="ZipCode">CEP, opcional.</param>
public sealed record CreatePersonRequest(
    string Name,
    DateOnly BirthDate,
    string? Cns,
    string? Cpf,
    string? MotherName,
    string? Gender,
    string? RaceColor,
    string? Phone,
    string? Email,
    string? Street,
    string? Number,
    string? Neighborhood,
    string? City,
    string? State,
    string? ZipCode);
