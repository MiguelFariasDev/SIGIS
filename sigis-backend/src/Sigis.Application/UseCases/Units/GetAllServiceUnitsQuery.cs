using MediatR;
using Sigis.Domain.Abstractions;

namespace Sigis.Application.UseCases.Units;

/// <summary>Consulta que lista todas as unidades de serviço da rede.</summary>
public sealed record GetAllServiceUnitsQuery : IRequest<Result<IReadOnlyList<ServiceUnitResponse>>>;
