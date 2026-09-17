namespace Sigis.Application.UseCases.Indicators;

/// <summary>
/// Painel de indicadores operacionais do SIGIS, agregados a partir de fila,
/// atendimentos e encaminhamentos — filtrados opcionalmente por unidade e
/// período. Cada métrica é derivada diretamente de campos persistidos
/// (nenhum valor é estimado ou inventado).
/// </summary>
/// <param name="TotalNaFilaAtiva">Total de entradas de fila em situação ativa (aguardando ou em atendimento).</param>
/// <param name="FilaPorStatus">Quantidade de entradas de fila por situação (<c>QueueStatus</c>).</param>
/// <param name="FilaPorPrioridade">Quantidade de entradas de fila por prioridade (<c>QueuePriority</c>).</param>
/// <param name="TempoMedioEsperaMinutos">
/// Tempo médio, em minutos, entre a entrada na fila e a conclusão do
/// atendimento (<c>EnteredAt</c> → <c>UpdatedAt</c> das entradas concluídas
/// no filtro) — <see langword="null"/> quando não houver entradas concluídas
/// no período/unidade filtrados.
/// </param>
/// <param name="TotalAtendimentos">Total de atendimentos no filtro.</param>
/// <param name="AtendimentosPorStatus">Quantidade de atendimentos por situação de comparecimento (<c>AttendanceStatus</c>).</param>
/// <param name="TotalEncaminhamentos">Total de encaminhamentos no filtro (por unidade de origem ou destino).</param>
/// <param name="EncaminhamentosPorStatus">Quantidade de encaminhamentos por situação (<c>ReferralStatus</c>).</param>
public sealed record IndicatorsResponse(
    int TotalNaFilaAtiva,
    IReadOnlyDictionary<string, int> FilaPorStatus,
    IReadOnlyDictionary<string, int> FilaPorPrioridade,
    double? TempoMedioEsperaMinutos,
    int TotalAtendimentos,
    IReadOnlyDictionary<string, int> AtendimentosPorStatus,
    int TotalEncaminhamentos,
    IReadOnlyDictionary<string, int> EncaminhamentosPorStatus);
