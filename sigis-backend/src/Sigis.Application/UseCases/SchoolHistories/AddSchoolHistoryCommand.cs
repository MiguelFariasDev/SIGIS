using MediatR;
using Sigis.Domain.Abstractions;

namespace Sigis.Application.UseCases.SchoolHistories;

/// <summary>Comando para registrar um novo vínculo escolar de uma pessoa.</summary>
/// <param name="PersonId">Identificador da pessoa.</param>
/// <param name="SchoolName">Nome da escola.</param>
/// <param name="Grade">Série cursada.</param>
/// <param name="SchoolYear">Ano letivo.</param>
/// <param name="Shift">Turno, opcional.</param>
/// <param name="ClassGroup">Turma, opcional.</param>
/// <param name="StartDate">Data de início na escola, opcional.</param>
/// <param name="Notes">Observações, opcionais.</param>
public sealed record AddSchoolHistoryCommand(
    Guid PersonId,
    string? SchoolName,
    string? Grade,
    int SchoolYear,
    string? Shift,
    string? ClassGroup,
    DateOnly? StartDate,
    string? Notes) : IRequest<Result<SchoolHistoryResponse>>;
