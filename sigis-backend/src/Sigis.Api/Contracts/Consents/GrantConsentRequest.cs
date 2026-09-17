using Sigis.Domain.Enums;

namespace Sigis.Api.Contracts.Consents;

/// <summary>Requisição para conceder um novo consentimento LGPD para uma pessoa.</summary>
/// <param name="Type">Tipo/finalidade do consentimento.</param>
/// <param name="Version">Versão do termo de consentimento aceito.</param>
/// <param name="Evidence">Evidência da concessão (ex.: "presencial em unidade X"), opcional.</param>
/// <param name="GrantedByGuardianId">Identificador do responsável que concedeu, quando aplicável.</param>
public sealed record GrantConsentRequest(
    ConsentType Type,
    string Version,
    string? Evidence,
    Guid? GrantedByGuardianId);
