namespace Sigis.Application.UseCases.Units;

/// <summary>Resumo de uma unidade de serviço da rede, para exibição (ex.: seletor de destino de encaminhamento).</summary>
/// <param name="Id">Identificador da unidade.</param>
/// <param name="Name">Nome completo da unidade.</param>
/// <param name="Acronym">Sigla da unidade.</param>
/// <param name="Secretariat">Secretaria municipal responsável pela unidade.</param>
public sealed record ServiceUnitResponse(Guid Id, string Name, string Acronym, string Secretariat);
