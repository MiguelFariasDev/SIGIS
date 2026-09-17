using MediatR;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Enums;

namespace Sigis.Application.UseCases.Consents;

/// <summary>Comando para conceder um novo consentimento LGPD para uma pessoa.</summary>
/// <param name="PersonId">Identificador da pessoa.</param>
/// <param name="Type">Tipo/finalidade do consentimento.</param>
/// <param name="Version">Versão do termo de consentimento aceito.</param>
/// <param name="Evidence">Evidência da concessão, opcional.</param>
/// <param name="GrantedByGuardianId">Identificador do responsável que concedeu, quando aplicável.</param>
public sealed record GrantConsentCommand(
    Guid PersonId,
    ConsentType Type,
    string? Version,
    string? Evidence,
    Guid? GrantedByGuardianId) : IRequest<Result<PersonConsentResponse>>;
