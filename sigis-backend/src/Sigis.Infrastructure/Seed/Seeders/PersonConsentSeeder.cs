using Sigis.Domain.Entities;
using Sigis.Domain.Enums;

namespace Sigis.Infrastructure.Seed.Seeders;

/// <summary>Semeia os consentimentos LGPD granulares (29 no total).</summary>
public static class PersonConsentSeeder
{
    private const string TermVersion = "1.0";
    private const string DemoEvidence = "Assinatura digital no cadastro";

    /// <summary>
    /// Cria os consentimentos de demonstração: Clinical para todas as 12
    /// pessoas, Educational para 8, SocialAssistance para 6 (Ana Clara —
    /// caso 4 — é deliberadamente excluída, para bloquear seu encaminhamento
    /// ao CRASF) e Research para 3.
    /// </summary>
    /// <param name="persons">Pessoas já semeadas, indexadas pela chave curta do seed.</param>
    /// <param name="now">Instante de referência (UTC) de concessão dos consentimentos.</param>
    /// <returns>Os consentimentos criados.</returns>
    public static List<PersonConsent> Seed(IReadOnlyDictionary<string, Person> persons, DateTime now)
    {
        // Todas as 12 pessoas — consentimento clínico é a base mínima do cadastro.
        var clinical = persons.Keys.ToArray();

        // Educational (8) — inclui Ana Clara: seu bloqueio é especificamente em SocialAssistance.
        string[] educational =
            ["twin1", "twin2", "pedro", "anaClara", "lucas", "joaoPedro", "mariaEduarda", "luizGustavo"];

        // SocialAssistance (6) — Ana Clara deliberadamente ausente (caso 4).
        string[] socialAssistance = ["twin1", "twin2", "pedro", "lucas", "joaoPedro", "mariaEduarda"];

        // Research (3).
        string[] research = ["pedro", "rafael", "isabelly"];

        var consents = new List<PersonConsent>();
        AddConsents(consents, persons, clinical, ConsentType.Clinical, now);
        AddConsents(consents, persons, educational, ConsentType.Educational, now);
        AddConsents(consents, persons, socialAssistance, ConsentType.SocialAssistance, now);
        AddConsents(consents, persons, research, ConsentType.Research, now);

        return consents;
    }

    private static void AddConsents(
        List<PersonConsent> consents,
        IReadOnlyDictionary<string, Person> persons,
        IEnumerable<string> personKeys,
        ConsentType type,
        DateTime now)
    {
        foreach (var key in personKeys)
        {
            consents.Add(PersonConsent.Create(
                persons[key].Id, type, now.AddDays(-30), TermVersion, now.AddDays(-30), evidence: DemoEvidence).Value);
        }
    }
}
