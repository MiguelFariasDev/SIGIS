using MediatR;
using Sigis.Domain.Abstractions;

namespace Sigis.Application.UseCases.Duplicates;

/// <summary>Consulta um alerta de duplicidade pelo identificador.</summary>
/// <param name="Id">Identificador do alerta.</param>
public sealed record GetDuplicateByIdQuery(Guid Id) : IRequest<Result<DuplicateAlertResponse>>;
