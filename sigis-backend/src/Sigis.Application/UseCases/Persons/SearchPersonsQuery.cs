using MediatR;
using Sigis.Domain.Abstractions;

namespace Sigis.Application.UseCases.Persons;

/// <summary>Consulta de busca de pessoas por nome (parcial ou aproximado).</summary>
/// <param name="Term">Termo de busca por nome.</param>
/// <param name="Limit">Quantidade máxima de resultados retornados.</param>
public sealed record SearchPersonsQuery(string Term, int Limit) : IRequest<Result<IReadOnlyList<PersonSummary>>>;
