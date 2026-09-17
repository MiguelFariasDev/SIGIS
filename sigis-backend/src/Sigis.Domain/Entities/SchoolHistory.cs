using Sigis.Domain.Abstractions;
using Sigis.Domain.Abstractions.Errors;
using Sigis.Domain.Enums;

namespace Sigis.Domain.Entities;

/// <summary>
/// Registro de uma escola frequentada por uma pessoa ao longo do tempo
/// (NAPE, ficha A.3 — "histórico escolar"). Uma pessoa pode ter vários
/// registros (1:N), mas apenas um pode estar com <see cref="SchoolStatus.Ativo"/>.
/// </summary>
public sealed class SchoolHistory : Entity
{
    /// <summary>Identificador da pessoa.</summary>
    public Guid PersonId { get; private set; }

    /// <summary>Nome da escola.</summary>
    public string SchoolName { get; private set; }

    /// <summary>Série cursada.</summary>
    public string Grade { get; private set; }

    /// <summary>Turno (Manhã, Tarde, Noite, Integral), quando informado.</summary>
    public string? Shift { get; private set; }

    /// <summary>Turma (ex.: "A", "B"), quando informada.</summary>
    public string? ClassGroup { get; private set; }

    /// <summary>Ano letivo.</summary>
    public int SchoolYear { get; private set; }

    /// <summary>Data de início na escola, quando informada.</summary>
    public DateOnly? StartDate { get; private set; }

    /// <summary>Data de término/saída da escola, quando informada.</summary>
    public DateOnly? EndDate { get; private set; }

    /// <summary>Situação atual do registro escolar.</summary>
    public SchoolStatus Status { get; private set; }

    /// <summary>Observações, quando informadas.</summary>
    public string? Notes { get; private set; }

    /// <summary>Data e hora (UTC) de criação do registro.</summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>Identificador do profissional que cadastrou o registro.</summary>
    public Guid CreatedByProfessionalId { get; private set; }

    private SchoolHistory(
        Guid id, Guid personId, string schoolName, string grade, string? shift, string? classGroup,
        int schoolYear, DateOnly? startDate, DateTime createdAt, Guid createdByProfessionalId, string? notes)
    {
        Id = id;
        PersonId = personId;
        SchoolName = schoolName;
        Grade = grade;
        Shift = shift;
        ClassGroup = classGroup;
        SchoolYear = schoolYear;
        StartDate = startDate;
        Status = SchoolStatus.Ativo;
        Notes = notes;
        CreatedAt = createdAt;
        CreatedByProfessionalId = createdByProfessionalId;
    }

    /// <summary>
    /// Cria um novo registro de histórico escolar, sempre com situação
    /// inicial <see cref="SchoolStatus.Ativo"/> e sem data de término.
    /// </summary>
    /// <param name="personId">Identificador da pessoa.</param>
    /// <param name="schoolName">Nome da escola.</param>
    /// <param name="grade">Série cursada.</param>
    /// <param name="schoolYear">Ano letivo.</param>
    /// <param name="createdByProfessionalId">Identificador do profissional que está cadastrando.</param>
    /// <param name="createdAt">Data e hora (UTC) de criação.</param>
    /// <param name="currentYear">Ano corrente, usado para validar <paramref name="schoolYear"/> (RN-SH03).</param>
    /// <param name="shift">Turno, opcional.</param>
    /// <param name="classGroup">Turma, opcional.</param>
    /// <param name="startDate">Data de início na escola, opcional.</param>
    /// <param name="notes">Observações, opcionais.</param>
    /// <returns>
    /// Um <see cref="Result{T}"/> de sucesso, ou de falha quando nome da
    /// escola, série ou ano letivo forem inválidos.
    /// </returns>
    /// <remarks>
    /// RN-SH01 (só um registro ativo por pessoa) depende de consultar outros
    /// registros já persistidos e é responsabilidade da camada de aplicação
    /// antes de chamar este método — reforçada pelo índice único parcial em
    /// <c>(person_id) WHERE status = 'Ativo'</c> no banco.
    /// </remarks>
    public static Result<SchoolHistory> Create(
        Guid personId,
        string? schoolName,
        string? grade,
        int schoolYear,
        Guid createdByProfessionalId,
        DateTime createdAt,
        int currentYear,
        string? shift = null,
        string? classGroup = null,
        DateOnly? startDate = null,
        string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(schoolName))
            return Result<SchoolHistory>.Failure(SchoolHistoryErrors.NomeEscolaObrigatorio);

        if (string.IsNullOrWhiteSpace(grade))
            return Result<SchoolHistory>.Failure(SchoolHistoryErrors.SerieObrigatoria);

        if (schoolYear is < 1900 || schoolYear > currentYear)
            return Result<SchoolHistory>.Failure(SchoolHistoryErrors.AnoLetivoInvalido);

        return Result<SchoolHistory>.Success(new SchoolHistory(
            Guid.NewGuid(), personId, schoolName.Trim(), grade.Trim(), shift?.Trim(), classGroup?.Trim(),
            schoolYear, startDate, createdAt, createdByProfessionalId, notes?.Trim()));
    }

    /// <summary>
    /// Encerra o vínculo com a escola (transferência, conclusão ou evasão),
    /// aplicando RN-SH02 (data de término só com status diferente de Ativo)
    /// e RN-SH04 (data de término posterior à de início).
    /// </summary>
    /// <param name="status">Nova situação (não pode ser <see cref="SchoolStatus.Ativo"/> se houver data de término).</param>
    /// <param name="endDate">Data de término, opcional.</param>
    /// <returns>
    /// Um <see cref="Result"/> de sucesso, ou de falha quando a data de
    /// término for informada com status Ativo ou for anterior/igual à data
    /// de início.
    /// </returns>
    public Result Close(SchoolStatus status, DateOnly? endDate)
    {
        if (status == SchoolStatus.Ativo && endDate.HasValue)
            return Result.Failure(SchoolHistoryErrors.DataTerminoApenasQuandoInativo);

        if (endDate.HasValue && StartDate.HasValue && endDate.Value <= StartDate.Value)
            return Result.Failure(SchoolHistoryErrors.DataTerminoInvalida);

        Status = status;
        EndDate = endDate;
        return Result.Success();
    }

    /// <summary>Indica se este é o registro escolar ativo da pessoa.</summary>
    /// <returns><see langword="true"/> quando <see cref="Status"/> é <see cref="SchoolStatus.Ativo"/>.</returns>
    public bool IsActive() => Status == SchoolStatus.Ativo;
}
