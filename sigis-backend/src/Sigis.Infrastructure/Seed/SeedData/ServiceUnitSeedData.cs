using Sigis.Domain.Enums;

namespace Sigis.Infrastructure.Seed.SeedData;

/// <summary>Dado estático de uma unidade de serviço a ser semeada.</summary>
/// <param name="Acronym">Sigla da unidade — usada como chave de referência pelos demais seeders.</param>
/// <param name="Name">Nome completo da unidade.</param>
/// <param name="Secretariat">Secretaria municipal responsável.</param>
public sealed record ServiceUnitSeed(string Acronym, string Name, ResponsibleSecretariat Secretariat);

/// <summary>As 5 unidades de serviço da rede municipal usadas na demonstração.</summary>
public static class ServiceUnitSeedData
{
    /// <summary>Unidades a semear.</summary>
    public static readonly IReadOnlyList<ServiceUnitSeed> Units =
    [
        new("NASF", "Núcleo Ampliado de Saúde da Família", ResponsibleSecretariat.Health),
        new("CREAES", "Centro de Referência em Atenção Especializada em Saúde", ResponsibleSecretariat.Health),
        new("NAPE", "Núcleo de Atendimento Pedagógico Especializado", ResponsibleSecretariat.Education),
        new("CMA", "Casa Mais Azul", ResponsibleSecretariat.Health),
        new("CRASF", "Centro de Referência de Assistência Social e Familiar", ResponsibleSecretariat.SocialAssistance),
    ];
}
