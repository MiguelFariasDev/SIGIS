namespace Sigis.Api.Contracts.Consents;

/// <summary>Requisição para revogar um consentimento LGPD já concedido.</summary>
/// <param name="Justification">
/// Justificativa da revogação, exigida para fins de auditoria — o SIGIS
/// ainda não possui um registro de auditoria dedicado a revogações; a
/// justificativa é validada mas não persistida nesta versão.
/// </param>
public sealed record RevokeConsentRequest(string Justification);
