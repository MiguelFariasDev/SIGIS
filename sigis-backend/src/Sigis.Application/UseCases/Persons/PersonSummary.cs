namespace Sigis.Application.UseCases.Persons;

/// <summary>Resumo leve de uma pessoa, usado em listas de busca e de candidatos a duplicidade.</summary>
/// <param name="Id">Identificador da pessoa.</param>
/// <param name="Name">Nome completo.</param>
/// <param name="BirthDate">Data de nascimento.</param>
/// <param name="MotherName">Nome da mãe, quando informado — ajuda a diferenciar homônimos.</param>
public sealed record PersonSummary(Guid Id, string Name, DateOnly BirthDate, string? MotherName);
