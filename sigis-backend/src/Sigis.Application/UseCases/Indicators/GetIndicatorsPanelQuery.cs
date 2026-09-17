using MediatR;
using Sigis.Domain.Abstractions;

namespace Sigis.Application.UseCases.Indicators;

/// <summary>
/// Consulta o painel de indicadores operacionais (fila, atendimentos,
/// encaminhamentos), filtrado opcionalmente por unidade e período.
/// </summary>
/// <param name="UnitId">Identificador da unidade de serviço, opcional (filtra fila/atendimentos por unidade; encaminhamentos por origem ou destino).</param>
/// <param name="From">Início do período (UTC, inclusive), opcional.</param>
/// <param name="To">Fim do período (UTC, inclusive), opcional.</param>
public sealed record GetIndicatorsPanelQuery(Guid? UnitId, DateTime? From, DateTime? To)
    : IRequest<Result<IndicatorsResponse>>;
