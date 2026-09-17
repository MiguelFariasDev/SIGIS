using DayOfWeek = Sigis.Domain.Enums.DayOfWeek;

namespace Sigis.Application.UseCases.ConcurrentTreatments;

/// <summary>Representação de um atendimento concomitante de uma pessoa retornada pela API.</summary>
public sealed record ConcurrentTreatmentResponse(
    Guid Id,
    Guid PersonId,
    string Specialty,
    string Location,
    string ProfessionalName,
    DayOfWeek DayOfWeek,
    TimeOnly StartTime,
    TimeOnly EndTime,
    string? Notes,
    DateTime CreatedAt,
    DateTime UpdatedAt);
