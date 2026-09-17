namespace Sigis.Application.UseCases.Indicators;

/// <summary>Resumo da fila de uma unidade de serviço, para o gráfico de fila por serviço do painel.</summary>
/// <param name="Servico">Sigla da unidade de serviço.</param>
/// <param name="Aguardando">Quantidade de entradas em situação <c>Waiting</c>.</param>
/// <param name="EmAtendimento">Quantidade de entradas em situação <c>InAttendance</c>.</param>
public sealed record FilaPorServicoResponse(string Servico, int Aguardando, int EmAtendimento);
