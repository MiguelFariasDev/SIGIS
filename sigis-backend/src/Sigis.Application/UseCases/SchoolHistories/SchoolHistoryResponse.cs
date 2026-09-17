using Sigis.Domain.Enums;

namespace Sigis.Application.UseCases.SchoolHistories;

/// <summary>Representação de um registro de histórico escolar retornada pela API.</summary>
/// <param name="Id">Identificador do registro.</param>
/// <param name="PersonId">Identificador da pessoa.</param>
/// <param name="SchoolName">Nome da escola.</param>
/// <param name="Grade">Série cursada.</param>
/// <param name="Shift">Turno, quando informado.</param>
/// <param name="ClassGroup">Turma, quando informada.</param>
/// <param name="SchoolYear">Ano letivo.</param>
/// <param name="StartDate">Data de início na escola, quando informada.</param>
/// <param name="EndDate">Data de término/saída da escola, quando informada.</param>
/// <param name="Status">Situação atual do registro escolar.</param>
/// <param name="Notes">Observações, quando informadas.</param>
/// <param name="CreatedAt">Data e hora (UTC) de criação do registro.</param>
/// <param name="CreatedByProfessionalId">Identificador do profissional que cadastrou o registro.</param>
public sealed record SchoolHistoryResponse(
    Guid Id,
    Guid PersonId,
    string SchoolName,
    string Grade,
    string? Shift,
    string? ClassGroup,
    int SchoolYear,
    DateOnly? StartDate,
    DateOnly? EndDate,
    SchoolStatus Status,
    string? Notes,
    DateTime CreatedAt,
    Guid CreatedByProfessionalId);
