using MediatR;
using Sigis.Domain.Abstractions;

namespace Sigis.Application.UseCases.Indicators;

/// <summary>Consulta os alertas recentes (duplicidades pendentes e entradas em busca ativa) para o painel de indicadores.</summary>
public sealed record GetAlertasRecentesQuery : IRequest<Result<IReadOnlyList<AlertaRecenteResponse>>>;
