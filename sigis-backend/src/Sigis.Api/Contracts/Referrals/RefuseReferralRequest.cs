namespace Sigis.Api.Contracts.Referrals;

/// <summary>Requisição de recusa de um encaminhamento.</summary>
/// <param name="Motivo">Motivo da recusa.</param>
public sealed record RefuseReferralRequest(string? Motivo);
