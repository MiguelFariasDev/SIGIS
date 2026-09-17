using MediatR;
using Sigis.Domain.Abstractions;

namespace Sigis.Application.UseCases.Indicators;

/// <summary>Consulta a quantidade de atendimentos por dia, no período informado, para o painel de indicadores.</summary>
/// <param name="Periodo">Período: <c>"HOJE"</c>, <c>"7D"</c> ou <c>"30D"</c> — inválido/ausente cai em <c>"7D"</c>.</param>
public sealed record GetAtendimentosPorDiaQuery(string? Periodo) : IRequest<Result<IReadOnlyList<AtendimentosPorDiaResponse>>>;
