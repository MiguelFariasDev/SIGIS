namespace Sigis.Application.UseCases.Family;

/// <summary>Representação da composição familiar de uma pessoa retornada pela API.</summary>
public sealed record FamilyCompositionResponse(
    Guid Id,
    Guid PersonId,
    string? FatherName,
    string? FatherEducation,
    string? FatherOccupation,
    string? MotherName,
    string? MotherEducation,
    string? MotherOccupation,
    int? SiblingsCount,
    string? SiblingsAges,
    int? HouseholdMembersCount,
    string? ParentsMaritalStatus,
    string? FiliationType,
    bool? PlannedPregnancy,
    int? PregnanciesCount,
    int? AbortionsCount,
    string? PregnancyHealthIssue,
    string? DeliveryType,
    string? MedicationDuringPregnancy,
    DateTime CreatedAt,
    DateTime UpdatedAt);
