namespace Sigis.Application.UseCases.Family;

/// <summary>Converte a entidade <see cref="Sigis.Domain.Entities.FamilyComposition"/> em <see cref="FamilyCompositionResponse"/>.</summary>
internal static class FamilyCompositionMapper
{
    /// <summary>Converte a entidade em sua representação de resposta.</summary>
    /// <param name="entity">Composição familiar a converter.</param>
    /// <returns>A resposta correspondente.</returns>
    public static FamilyCompositionResponse ToResponse(Sigis.Domain.Entities.FamilyComposition entity) => new(
        entity.Id, entity.PersonId, entity.FatherName, entity.FatherEducation, entity.FatherOccupation,
        entity.MotherName, entity.MotherEducation, entity.MotherOccupation, entity.SiblingsCount,
        entity.SiblingsAges, entity.HouseholdMembersCount, entity.ParentsMaritalStatus, entity.FiliationType,
        entity.PlannedPregnancy, entity.PregnanciesCount, entity.AbortionsCount, entity.PregnancyHealthIssue,
        entity.DeliveryType, entity.MedicationDuringPregnancy, entity.CreatedAt, entity.UpdatedAt);
}
