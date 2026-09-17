namespace Sigis.Api.Contracts.ClinicalProfile;

/// <summary>Requisição para criar ou atualizar o perfil clínico NASF de uma pessoa.</summary>
/// <param name="MedicalRecordNumber">Número do prontuário físico, opcional.</param>
/// <param name="ClinicalHypothesis">Hipótese diagnóstica (H.D.), opcional.</param>
/// <param name="ApsReferenceUnitId">Identificador da unidade APS de referência, opcional (RN-CP04: deve ser da Secretaria de Saúde).</param>
public sealed record UpdateClinicalProfileRequest(
    string? MedicalRecordNumber,
    string? ClinicalHypothesis,
    Guid? ApsReferenceUnitId);
