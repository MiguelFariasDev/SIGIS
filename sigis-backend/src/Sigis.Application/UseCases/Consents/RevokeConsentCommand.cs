using MediatR;
using Sigis.Domain.Abstractions;

namespace Sigis.Application.UseCases.Consents;

/// <summary>Comando para revogar um consentimento LGPD já concedido a uma pessoa.</summary>
/// <param name="PersonId">Identificador da pessoa.</param>
/// <param name="ConsentId">Identificador do consentimento a revogar.</param>
/// <param name="Justification">Justificativa da revogação (exigida, não persistida nesta versão).</param>
public sealed record RevokeConsentCommand(Guid PersonId, Guid ConsentId, string? Justification) : IRequest<Result>;
