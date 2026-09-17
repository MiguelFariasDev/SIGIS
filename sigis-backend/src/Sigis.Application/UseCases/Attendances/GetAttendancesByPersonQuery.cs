using MediatR;
using Sigis.Domain.Abstractions;

namespace Sigis.Application.UseCases.Attendances;

/// <summary>Lista o histórico de atendimentos de uma pessoa em todas as unidades da rede.</summary>
/// <param name="PersonId">Identificador da pessoa.</param>
public sealed record GetAttendancesByPersonQuery(Guid PersonId) : IRequest<Result<List<AttendanceResponse>>>;
