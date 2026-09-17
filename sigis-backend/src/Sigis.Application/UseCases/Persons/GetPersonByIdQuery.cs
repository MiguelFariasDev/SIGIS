using MediatR;
using Sigis.Domain.Abstractions;

namespace Sigis.Application.UseCases.Persons;

/// <summary>Consulta os dados completos de uma pessoa pelo identificador.</summary>
/// <param name="PersonId">Identificador da pessoa.</param>
public sealed record GetPersonByIdQuery(Guid PersonId) : IRequest<Result<PersonDetailResponse>>;
