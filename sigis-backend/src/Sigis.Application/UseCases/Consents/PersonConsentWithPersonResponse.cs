using Sigis.Domain.Enums;

namespace Sigis.Application.UseCases.Consents;

/// <summary>Consentimento LGPD com o nome da pessoa — item do painel global de consentimentos (DPO/coordenador).</summary>
public sealed record PersonConsentWithPersonResponse(
    Guid Id,
    Guid PersonId,
    string PersonName,
    ConsentType Type,
    bool Granted,
    DateTime GrantedAt,
    Guid? GrantedByGuardianId,
    DateTime? RevokedAt,
    string Version,
    string? Evidence,
    DateTime CreatedAt);
