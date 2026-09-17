namespace Sigis.Api.Contracts.Duplicates;

/// <summary>Requisição para marcar um alerta de duplicidade como falso positivo.</summary>
/// <param name="Note">Observação opcional do coordenador sobre a decisão.</param>
public sealed record MarkAsFalsePositiveRequest(string? Note);
