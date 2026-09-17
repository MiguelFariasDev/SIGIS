namespace Sigis.Application.UseCases.Development;

/// <summary>Converte a entidade <see cref="Sigis.Domain.Entities.DevelopmentMilestones"/> em <see cref="DevelopmentMilestonesResponse"/>.</summary>
internal static class DevelopmentMilestonesMapper
{
    /// <summary>Converte a entidade em sua representação de resposta.</summary>
    /// <param name="entity">Marcos de desenvolvimento a converter.</param>
    /// <returns>A resposta correspondente.</returns>
    public static DevelopmentMilestonesResponse ToResponse(Sigis.Domain.Entities.DevelopmentMilestones entity) => new(
        entity.Id, entity.PersonId, entity.AgeWalkedMonths, entity.AgeTalkedMonths, entity.LocomotionDifficulty,
        entity.CoordinationDifficulty, entity.VisualDifficulty, entity.HearingDifficulty, entity.SpeechProblems,
        entity.CommandComprehension, entity.CommunicationForm, entity.ManualDominance, entity.CreatedAt, entity.UpdatedAt);
}
