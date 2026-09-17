using MediatR;
using Sigis.Domain.Abstractions;

namespace Sigis.Application.UseCases.ClinicalProfile;

/// <summary>
/// Comando para criar (se ainda não existir) ou atualizar o perfil clínico
/// NASF de uma pessoa (relação 1:1).
/// </summary>
public sealed record UpdateClinicalProfileCommand(
    Guid PersonId,
    string? MedicalRecordNumber,
    string? ClinicalHypothesis,
    Guid? ApsReferenceUnitId) : IRequest<Result<PersonClinicalProfileResponse>>;
