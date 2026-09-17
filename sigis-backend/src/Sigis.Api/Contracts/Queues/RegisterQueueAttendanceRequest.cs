namespace Sigis.Api.Contracts.Queues;

/// <summary>Requisição de registro de comparecimento em uma entrada de fila.</summary>
/// <param name="Comparecimento">Situação de comparecimento: "COMPARECEU" ou "FALTOU".</param>
public sealed record RegisterQueueAttendanceRequest(string Comparecimento);
