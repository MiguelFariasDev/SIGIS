using MediatR;
using Sigis.Domain.Abstractions;

namespace Sigis.Application.UseCases.Development;

/// <summary>Consulta os marcos de desenvolvimento de uma pessoa.</summary>
/// <param name="PersonId">Identificador da pessoa.</param>
public sealed record GetDevelopmentMilestonesQuery(Guid PersonId) : IRequest<Result<DevelopmentMilestonesResponse>>;
