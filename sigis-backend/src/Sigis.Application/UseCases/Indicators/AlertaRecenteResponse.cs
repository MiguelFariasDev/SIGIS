namespace Sigis.Application.UseCases.Indicators;

/// <summary>Alerta recente para o painel de indicadores — duplicidade pendente ou entrada em busca ativa.</summary>
/// <param name="Id">Identificador do alerta de duplicidade ou da entrada de fila.</param>
/// <param name="Tipo"><c>"DUPLICIDADE"</c> ou <c>"BUSCA_ATIVA"</c>.</param>
/// <param name="Descricao">Descrição legível do alerta.</param>
/// <param name="DataHora">Data e hora (UTC) de referência do alerta.</param>
public sealed record AlertaRecenteResponse(Guid Id, string Tipo, string Descricao, DateTime DataHora);
