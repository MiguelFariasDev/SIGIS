using MediatR;
using Sigis.Domain.Abstractions;

namespace Sigis.Application.UseCases.Attendances;

/// <summary>Consulta um atendimento pelo identificador.</summary>
/// <param name="Id">Identificador do atendimento.</param>
public sealed record GetAttendanceByIdQuery(Guid Id) : IRequest<Result<AttendanceResponse>>;
