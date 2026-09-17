using Sigis.Domain.Entities;
using Sigis.Domain.Interfaces;

namespace Sigis.Application.UseCases.Attendances;

/// <summary>Monta o <see cref="AttendanceResponse"/> de um atendimento, resolvendo pessoa, unidade e profissional.</summary>
public static class AttendanceResponseMapper
{
    /// <summary>Mapeia um <see cref="Attendance"/> para <see cref="AttendanceResponse"/>.</summary>
    public static async Task<AttendanceResponse> MapAsync(
        Attendance attendance,
        IPersonRepository personRepository,
        IServiceUnitRepository serviceUnitRepository,
        IProfessionalRepository professionalRepository,
        CancellationToken cancellationToken)
    {
        var person = await personRepository.GetByIdAsync(attendance.PersonId, cancellationToken);
        var unit = await serviceUnitRepository.GetByIdAsync(attendance.UnitId, cancellationToken);
        var professional = await professionalRepository.GetByIdAsync(attendance.ProfessionalId, cancellationToken);

        return new AttendanceResponse(
            attendance.Id,
            attendance.PersonId,
            person?.Name.Value ?? "Pessoa não encontrada",
            attendance.UnitId,
            unit?.Acronym ?? "?",
            attendance.ProfessionalId,
            professional?.Name.Value ?? "Profissional não encontrado",
            attendance.DateTime,
            attendance.SessionType.ToString(),
            attendance.SessionNumber,
            attendance.Status.ToString(),
            attendance.FormData,
            attendance.TriagedByProfessionalId,
            attendance.MainComplaint,
            attendance.CreatedAt);
    }
}
