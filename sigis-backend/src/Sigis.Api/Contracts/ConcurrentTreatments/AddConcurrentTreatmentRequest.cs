using DayOfWeek = Sigis.Domain.Enums.DayOfWeek;

namespace Sigis.Api.Contracts.ConcurrentTreatments;

/// <summary>Requisição para registrar um atendimento concomitante de uma pessoa.</summary>
/// <param name="Specialty">Especialidade do profissional.</param>
/// <param name="Location">Local do atendimento.</param>
/// <param name="ProfessionalName">Nome do profissional responsável.</param>
/// <param name="DayOfWeek">Dia da semana do atendimento.</param>
/// <param name="StartTime">Horário de início.</param>
/// <param name="EndTime">Horário de término.</param>
/// <param name="Notes">Observações, opcionais.</param>
public sealed record AddConcurrentTreatmentRequest(
    string Specialty,
    string Location,
    string ProfessionalName,
    DayOfWeek DayOfWeek,
    TimeOnly StartTime,
    TimeOnly EndTime,
    string? Notes);
