namespace Sigis.Application.UseCases.Development;

/// <summary>Representação dos marcos de desenvolvimento de uma pessoa retornada pela API.</summary>
public sealed record DevelopmentMilestonesResponse(
    Guid Id,
    Guid PersonId,
    int? AgeWalkedMonths,
    int? AgeTalkedMonths,
    bool? LocomotionDifficulty,
    bool? CoordinationDifficulty,
    bool? VisualDifficulty,
    bool? HearingDifficulty,
    string? SpeechProblems,
    string? CommandComprehension,
    string? CommunicationForm,
    string? ManualDominance,
    DateTime CreatedAt,
    DateTime UpdatedAt);
