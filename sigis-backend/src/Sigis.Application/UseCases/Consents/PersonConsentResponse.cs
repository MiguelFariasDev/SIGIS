using Sigis.Domain.Enums;

namespace Sigis.Application.UseCases.Consents;

/// <summary>Representação de um consentimento LGPD de uma pessoa retornada pela API.</summary>
public sealed record PersonConsentResponse(
    Guid Id,
    Guid PersonId,
    ConsentType Type,
    bool Granted,
    DateTime GrantedAt,
    Guid? GrantedByGuardianId,
    DateTime? RevokedAt,
    string Version,
    string? Evidence,
    DateTime CreatedAt);
