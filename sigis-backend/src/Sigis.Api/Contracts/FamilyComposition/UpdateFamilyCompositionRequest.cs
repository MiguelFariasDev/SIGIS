namespace Sigis.Api.Contracts.FamilyComposition;

/// <summary>
/// Requisição para criar ou atualizar a composição familiar de uma pessoa.
/// Todos os campos são opcionais — cada seção da ficha pode ser preenchida
/// aos poucos.
/// </summary>
public sealed record UpdateFamilyCompositionRequest(
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
    string? MedicationDuringPregnancy);
