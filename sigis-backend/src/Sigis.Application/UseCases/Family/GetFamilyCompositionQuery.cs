using MediatR;
using Sigis.Domain.Abstractions;

namespace Sigis.Application.UseCases.Family;

/// <summary>Consulta a composição familiar de uma pessoa.</summary>
/// <param name="PersonId">Identificador da pessoa.</param>
public sealed record GetFamilyCompositionQuery(Guid PersonId) : IRequest<Result<FamilyCompositionResponse>>;
