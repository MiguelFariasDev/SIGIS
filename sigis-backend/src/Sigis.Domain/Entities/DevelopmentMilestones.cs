using Sigis.Domain.Abstractions;
using Sigis.Domain.Abstractions.Errors;

namespace Sigis.Domain.Entities;

/// <summary>
/// Marcos do desenvolvimento motor, perceptivo e de linguagem de uma pessoa
/// (NAPE, fichas A.2 §5-6 e A.3 §6-7). Relação 1:1 com <see cref="Person"/>.
/// </summary>
public sealed class DevelopmentMilestones : Entity
{
    private const int MinAgeMonths = 0;
    private const int MaxAgeMonths = 120;
    private static readonly string[] ValidManualDominances = ["Destro", "Canhoto", "Ambidestro"];

    /// <summary>Identificador da pessoa.</summary>
    public Guid PersonId { get; private set; }

    /// <summary>Idade (em meses) em que a pessoa começou a andar, quando informada.</summary>
    public int? AgeWalkedMonths { get; private set; }

    /// <summary>Idade (em meses) em que a pessoa começou a falar, quando informada.</summary>
    public int? AgeTalkedMonths { get; private set; }

    /// <summary>Indica dificuldade de locomoção, quando informado.</summary>
    public bool? LocomotionDifficulty { get; private set; }

    /// <summary>Indica dificuldade de coordenação, quando informado.</summary>
    public bool? CoordinationDifficulty { get; private set; }

    /// <summary>Indica dificuldade visual, quando informado.</summary>
    public bool? VisualDifficulty { get; private set; }

    /// <summary>Indica dificuldade auditiva, quando informado.</summary>
    public bool? HearingDifficulty { get; private set; }

    /// <summary>Problemas de fala relatados, quando informados.</summary>
    public string? SpeechProblems { get; private set; }

    /// <summary>Compreensão de comandos, quando informada.</summary>
    public string? CommandComprehension { get; private set; }

    /// <summary>Forma de comunicação predominante, quando informada.</summary>
    public string? CommunicationForm { get; private set; }

    /// <summary>Dominância manual ("Destro", "Canhoto" ou "Ambidestro"), quando informada.</summary>
    public string? ManualDominance { get; private set; }

    /// <summary>Data e hora (UTC) de criação do registro.</summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>Data e hora (UTC) da última atualização do registro.</summary>
    public DateTime UpdatedAt { get; private set; }

    private DevelopmentMilestones(Guid id, Guid personId, DateTime createdAt)
    {
        Id = id;
        PersonId = personId;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }

    /// <summary>
    /// Cria o registro de marcos de desenvolvimento de uma pessoa. Todos os
    /// campos além de <paramref name="personId"/> são opcionais.
    /// </summary>
    /// <param name="personId">Identificador da pessoa (único — relação 1:1).</param>
    /// <param name="createdAt">Data e hora (UTC) de criação.</param>
    /// <param name="ageWalkedMonths">Idade em que andou, em meses (RN-DM02: 0 a 120).</param>
    /// <param name="ageTalkedMonths">Idade em que falou, em meses (RN-DM02: 0 a 120).</param>
    /// <param name="locomotionDifficulty">Dificuldade de locomoção, opcional.</param>
    /// <param name="coordinationDifficulty">Dificuldade de coordenação, opcional.</param>
    /// <param name="visualDifficulty">Dificuldade visual, opcional.</param>
    /// <param name="hearingDifficulty">Dificuldade auditiva, opcional.</param>
    /// <param name="speechProblems">Problemas de fala, opcionais.</param>
    /// <param name="commandComprehension">Compreensão de comandos, opcional.</param>
    /// <param name="communicationForm">Forma de comunicação, opcional.</param>
    /// <param name="manualDominance">Dominância manual (RN-DM03), opcional.</param>
    /// <returns>
    /// Um <see cref="Result{T}"/> de sucesso, ou de falha quando as idades
    /// estiverem fora do intervalo válido ou a dominância manual for inválida.
    /// </returns>
    public static Result<DevelopmentMilestones> Create(
        Guid personId,
        DateTime createdAt,
        int? ageWalkedMonths = null,
        int? ageTalkedMonths = null,
        bool? locomotionDifficulty = null,
        bool? coordinationDifficulty = null,
        bool? visualDifficulty = null,
        bool? hearingDifficulty = null,
        string? speechProblems = null,
        string? commandComprehension = null,
        string? communicationForm = null,
        string? manualDominance = null)
    {
        var validation = Validate(ageWalkedMonths, ageTalkedMonths, manualDominance);
        if (validation.IsFailure)
            return Result<DevelopmentMilestones>.Failure(validation.Error);

        var milestones = new DevelopmentMilestones(Guid.NewGuid(), personId, createdAt)
        {
            AgeWalkedMonths = ageWalkedMonths,
            AgeTalkedMonths = ageTalkedMonths,
            LocomotionDifficulty = locomotionDifficulty,
            CoordinationDifficulty = coordinationDifficulty,
            VisualDifficulty = visualDifficulty,
            HearingDifficulty = hearingDifficulty,
            SpeechProblems = speechProblems?.Trim(),
            CommandComprehension = commandComprehension?.Trim(),
            CommunicationForm = communicationForm?.Trim(),
            ManualDominance = manualDominance?.Trim(),
        };

        return Result<DevelopmentMilestones>.Success(milestones);
    }

    /// <summary>Atualiza os marcos de desenvolvimento registrados.</summary>
    /// <returns>
    /// Um <see cref="Result"/> de sucesso, ou de falha quando as idades
    /// estiverem fora do intervalo válido ou a dominância manual for inválida.
    /// </returns>
    public Result UpdateData(
        int? ageWalkedMonths, int? ageTalkedMonths, bool? locomotionDifficulty, bool? coordinationDifficulty,
        bool? visualDifficulty, bool? hearingDifficulty, string? speechProblems, string? commandComprehension,
        string? communicationForm, string? manualDominance)
    {
        var validation = Validate(ageWalkedMonths, ageTalkedMonths, manualDominance);
        if (validation.IsFailure)
            return validation;

        AgeWalkedMonths = ageWalkedMonths;
        AgeTalkedMonths = ageTalkedMonths;
        LocomotionDifficulty = locomotionDifficulty;
        CoordinationDifficulty = coordinationDifficulty;
        VisualDifficulty = visualDifficulty;
        HearingDifficulty = hearingDifficulty;
        SpeechProblems = speechProblems?.Trim();
        CommandComprehension = commandComprehension?.Trim();
        CommunicationForm = communicationForm?.Trim();
        ManualDominance = manualDominance?.Trim();
        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }

    private static Result Validate(int? ageWalkedMonths, int? ageTalkedMonths, string? manualDominance)
    {
        if (ageWalkedMonths is < MinAgeMonths or > MaxAgeMonths)
            return Result.Failure(DevelopmentMilestonesErrors.IdadeInvalida);

        if (ageTalkedMonths is < MinAgeMonths or > MaxAgeMonths)
            return Result.Failure(DevelopmentMilestonesErrors.IdadeInvalida);

        if (manualDominance is not null && !ValidManualDominances.Contains(manualDominance))
            return Result.Failure(DevelopmentMilestonesErrors.DominanciaManualInvalida);

        return Result.Success();
    }
}
