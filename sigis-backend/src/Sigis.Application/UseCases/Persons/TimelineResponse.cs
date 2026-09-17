namespace Sigis.Application.UseCases.Persons;

/// <summary>Linha do tempo consolidada de atendimentos e encaminhamentos de uma pessoa em toda a rede (RF08).</summary>
/// <param name="PersonId">Identificador da pessoa.</param>
/// <param name="Level">Nível de detalhe efetivamente aplicado ("metadados" ou "completo").</param>
/// <param name="Entries">Eventos da linha do tempo, em ordem cronológica decrescente.</param>
public sealed record TimelineResponse(Guid PersonId, string Level, IReadOnlyList<TimelineEntry> Entries);

/// <summary>Um evento da linha do tempo de uma pessoa.</summary>
/// <param name="DateTime">Data e hora do evento.</param>
/// <param name="Type">Tipo do evento ("Atendimento" ou "Encaminhamento").</param>
/// <param name="UnitName">Nome da unidade de serviço associada ao evento.</param>
/// <param name="Details">
/// Detalhe do evento (motivo, queixa principal), presente apenas quando o
/// nível de disclosure solicitado é "completo" — em "metadados", é sempre <see langword="null"/>.
/// </param>
public sealed record TimelineEntry(DateTime DateTime, string Type, string UnitName, string? Details);
