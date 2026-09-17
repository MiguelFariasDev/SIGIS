using MediatR;
using Sigis.Domain.Abstractions;
using DayOfWeek = Sigis.Domain.Enums.DayOfWeek;

namespace Sigis.Application.UseCases.ConcurrentTreatments;

/// <summary>Comando para registrar um atendimento concomitante de uma pessoa.</summary>
/// <param name="PersonId">Identificador da pessoa.</param>
/// <param name="Specialty">Especialidade do profissional.</param>
/// <param name="Location">Local do atendimento.</param>
/// <param name="ProfessionalName">Nome do profissional responsável.</param>
/// <param name="DayOfWeek">Dia da semana do atendimento.</param>
/// <param name="StartTime">Horário de início.</param>
/// <param name="EndTime">Horário de término.</param>
/// <param name="Notes">Observações, opcionais.</param>
public sealed record AddConcurrentTreatmentCommand(
    Guid PersonId,
    string? Specialty,
    string? Location,
    string? ProfessionalName,
    DayOfWeek DayOfWeek,
    TimeOnly StartTime,
    TimeOnly EndTime,
    string? Notes) : IRequest<Result<ConcurrentTreatmentResponse>>;
