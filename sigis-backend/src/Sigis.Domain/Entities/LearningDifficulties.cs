using Sigis.Domain.Abstractions;
using Sigis.Domain.Abstractions.Errors;
using Sigis.Domain.Enums;

namespace Sigis.Domain.Entities;

/// <summary>
/// Dificuldade de aprendizagem identificada na sondagem psicopedagógica de
/// uma pessoa (NAPE, ficha A.3 §5). Relação 1:N com <see cref="Person"/> —
/// uma pessoa pode ter várias dificuldades, mas nunca duas do mesmo
/// <see cref="LearningDifficultyType"/>.
/// </summary>
public sealed class LearningDifficulties : Entity
{
    private static readonly string[] ValidSeverities = ["Leve", "Moderada", "Severa"];

    /// <summary>Identificador da pessoa.</summary>
    public Guid PersonId { get; private set; }

    /// <summary>Tipo de dificuldade de aprendizagem.</summary>
    public LearningDifficultyType Type { get; private set; }

    /// <summary>Gravidade ("Leve", "Moderada" ou "Severa"), quando informada.</summary>
    public string? Severity { get; private set; }

    /// <summary>Data em que a sondagem psicopedagógica foi aplicada, quando informada.</summary>
    public DateOnly? AssessmentDate { get; private set; }

    /// <summary>Observações, quando informadas.</summary>
    public string? Notes { get; private set; }

    /// <summary>Data e hora (UTC) de criação do registro.</summary>
    public DateTime CreatedAt { get; private set; }

    private LearningDifficulties(Guid id, Guid personId, LearningDifficultyType type, DateTime createdAt)
    {
        Id = id;
        PersonId = personId;
        Type = type;
        CreatedAt = createdAt;
    }

    /// <summary>
    /// Registra uma dificuldade de aprendizagem para a pessoa.
    /// </summary>
    /// <param name="personId">Identificador da pessoa.</param>
    /// <param name="type">Tipo de dificuldade.</param>
    /// <param name="createdAt">Data e hora (UTC) de criação.</param>
    /// <param name="today">Data de referência (RN-LD03), injetada para testabilidade.</param>
    /// <param name="severity">Gravidade (RN-LD02), opcional.</param>
    /// <param name="assessmentDate">Data da sondagem (RN-LD03: não pode ser futura), opcional.</param>
    /// <param name="notes">Observações, opcionais.</param>
    /// <returns>
    /// Um <see cref="Result{T}"/> de sucesso, ou de falha quando a gravidade
    /// for inválida ou a data da sondagem for futura.
    /// </returns>
    /// <remarks>
    /// RN-LD01 (um registro por tipo por pessoa) depende de consultar outros
    /// registros já persistidos e é reforçada pelo índice único
    /// <c>(person_id, type)</c> no banco — não é responsabilidade desta
    /// entidade isolada.
    /// </remarks>
    public static Result<LearningDifficulties> Create(
        Guid personId,
        LearningDifficultyType type,
        DateTime createdAt,
        DateOnly today,
        string? severity = null,
        DateOnly? assessmentDate = null,
        string? notes = null)
    {
        if (severity is not null && !ValidSeverities.Contains(severity))
            return Result<LearningDifficulties>.Failure(LearningDifficultiesErrors.GravidadeInvalida);

        if (assessmentDate.HasValue && assessmentDate.Value > today)
            return Result<LearningDifficulties>.Failure(LearningDifficultiesErrors.DataSondagemFutura);

        return Result<LearningDifficulties>.Success(new LearningDifficulties(Guid.NewGuid(), personId, type, createdAt)
        {
            Severity = severity?.Trim(),
            AssessmentDate = assessmentDate,
            Notes = notes?.Trim(),
        });
    }

    /// <summary>Atualiza a gravidade, a data da sondagem e as observações.</summary>
    /// <param name="today">Data de referência (RN-LD03), injetada para testabilidade.</param>
    /// <returns>
    /// Um <see cref="Result"/> de sucesso, ou de falha quando a gravidade for
    /// inválida ou a data da sondagem for futura.
    /// </returns>
    public Result UpdateData(string? severity, DateOnly? assessmentDate, string? notes, DateOnly today)
    {
        if (severity is not null && !ValidSeverities.Contains(severity))
            return Result.Failure(LearningDifficultiesErrors.GravidadeInvalida);

        if (assessmentDate.HasValue && assessmentDate.Value > today)
            return Result.Failure(LearningDifficultiesErrors.DataSondagemFutura);

        Severity = severity?.Trim();
        AssessmentDate = assessmentDate;
        Notes = notes?.Trim();

        return Result.Success();
    }
}
