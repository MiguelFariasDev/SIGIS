namespace Sigis.Api.Contracts.SchoolHistory;

/// <summary>Requisição para registrar um novo vínculo escolar de uma pessoa.</summary>
/// <param name="SchoolName">Nome da escola.</param>
/// <param name="Grade">Série cursada.</param>
/// <param name="SchoolYear">Ano letivo.</param>
/// <param name="Shift">Turno, opcional.</param>
/// <param name="ClassGroup">Turma, opcional.</param>
/// <param name="StartDate">Data de início na escola, opcional.</param>
/// <param name="Notes">Observações, opcionais.</param>
public sealed record AddSchoolHistoryRequest(
    string SchoolName,
    string Grade,
    int SchoolYear,
    string? Shift,
    string? ClassGroup,
    DateOnly? StartDate,
    string? Notes);
