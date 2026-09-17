using Sigis.Domain.Entities;

namespace Sigis.Infrastructure.Seed.Seeders;

/// <summary>Semeia o alerta de duplicidade pendente (caso 1/2 — gêmeas de cadastro).</summary>
public static class DuplicateAlertSeeder
{
    /// <summary>Score de similaridade do par de cadastros gêmeos — bem acima do limiar mínimo (0,75).</summary>
    public const double TwinsSimilarityScore = 0.98;

    /// <summary>Cria o alerta de duplicidade entre os dois cadastros "Maria Silva Santos".</summary>
    /// <param name="persons">Pessoas já semeadas, indexadas pela chave curta do seed.</param>
    /// <param name="now">Instante de referência (UTC) de criação do alerta.</param>
    /// <returns>O alerta de duplicidade criado, pendente de revisão.</returns>
    public static DuplicateAlert Seed(IReadOnlyDictionary<string, Person> persons, DateTime now) =>
        DuplicateAlert.Create(persons["twin1"].Id, persons["twin2"].Id, TwinsSimilarityScore, now.AddHours(-1)).Value;
}
