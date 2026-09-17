using MediatR;
using Sigis.Domain.Abstractions;

namespace Sigis.Application.UseCases.Attendances;

/// <summary>Registra o comparecimento ou a falta de uma pessoa a um atendimento agendado.</summary>
/// <param name="AttendanceId">Identificador do atendimento.</param>
/// <param name="Comparecimento">Situação de comparecimento: "COMPARECEU" ou "FALTOU".</param>
/// <param name="MainComplaint">Queixa principal relatada, opcional — quando informada, registra a triagem com o profissional autenticado.</param>
public sealed record RegisterAttendanceComparecimentoCommand(Guid AttendanceId, string Comparecimento, string? MainComplaint)
    : IRequest<Result<AttendanceResponse>>;
