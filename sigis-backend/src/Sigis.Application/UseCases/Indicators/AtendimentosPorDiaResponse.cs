namespace Sigis.Application.UseCases.Indicators;

/// <summary>Quantidade de atendimentos em um dia, para o gráfico de série temporal do painel.</summary>
/// <param name="Data">Data (sem horário) dos atendimentos.</param>
/// <param name="Quantidade">Quantidade de atendimentos no dia.</param>
public sealed record AtendimentosPorDiaResponse(DateOnly Data, int Quantidade);
