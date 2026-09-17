namespace Sigis.Application.UseCases.ClinicalProfile;

/// <summary>Representação do perfil clínico NASF de uma pessoa retornada pela API.</summary>
public sealed record PersonClinicalProfileResponse(
    Guid Id,
    Guid PersonId,
    string? MedicalRecordNumber,
    string? ClinicalHypothesis,
    Guid? ApsReferenceUnitId,
    DateTime CreatedAt,
    DateTime UpdatedAt);
