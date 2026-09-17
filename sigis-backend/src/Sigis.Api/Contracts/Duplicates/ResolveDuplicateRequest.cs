namespace Sigis.Api.Contracts.Duplicates;

/// <summary>Requisição de resolução genérica de um alerta de duplicidade.</summary>
/// <param name="Acao">Ação escolhida: <c>"MESCLAR"</c> ou <c>"FALSO_POSITIVO"</c>.</param>
public sealed record ResolveDuplicateRequest(string Acao);
