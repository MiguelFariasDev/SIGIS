namespace Sigis.Api.Contracts.DevelopmentMilestones;

/// <summary>
/// Requisição para criar ou atualizar os marcos de desenvolvimento de uma
/// pessoa. Todos os campos são opcionais.
/// </summary>
public sealed record UpdateDevelopmentMilestonesRequest(
    int? AgeWalkedMonths,
    int? AgeTalkedMonths,
    bool? LocomotionDifficulty,
    bool? CoordinationDifficulty,
    bool? VisualDifficulty,
    bool? HearingDifficulty,
    string? SpeechProblems,
    string? CommandComprehension,
    string? CommunicationForm,
    string? ManualDominance);
