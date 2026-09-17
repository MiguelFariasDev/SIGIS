using Sigis.Domain.Entities;
using Sigis.Domain.Enums;

namespace Sigis.Infrastructure.Seed.Seeders;

/// <summary>Semeia os encaminhamentos entre unidades (3 no total, em 3 situações diferentes).</summary>
public static class ReferralSeeder
{
    /// <summary>
    /// Cria os 3 encaminhamentos de demonstração: NASF → NAPE para Pedro
    /// Henrique (concluído), NASF → CRASF para Ana Clara (pendente — bloqueado
    /// pela falta de consentimento de Assistência Social, caso 4), e
    /// NAPE → CMA para Lucas Gabriel (aceito, com a fila resultante na
    /// unidade de destino).
    /// </summary>
    /// <param name="persons">Pessoas já semeadas, indexadas pela chave curta do seed.</param>
    /// <param name="units">Unidades de serviço já semeadas, indexadas por sigla.</param>
    /// <param name="now">Instante de referência (UTC) para calcular datas relativas.</param>
    /// <returns>
    /// Os encaminhamentos criados e a entrada de fila resultante da
    /// aceitação do encaminhamento de Lucas Gabriel (a aceitação de um
    /// encaminhamento não cria fila automaticamente na camada de aplicação —
    /// isso é responsabilidade de quem chama, replicado aqui para a demonstração).
    /// </returns>
    public static (List<Referral> Referrals, List<QueueEntry> ResultingQueueEntries) Seed(
        IReadOnlyDictionary<string, Person> persons, IReadOnlyDictionary<string, ServiceUnit> units, DateTime now)
    {
        var referrals = new List<Referral>();
        var resultingQueueEntries = new List<QueueEntry>();

        // 1. NASF → NAPE (Pedro Henrique) — concluído.
        var pedroReferral = Referral.Create(
            persons["pedro"].Id, units["NASF"].Id, units["NAPE"].Id,
            "Encaminhamento para avaliação psicopedagógica complementar", QueuePriority.ShortTerm,
            now.AddMonths(-4)).Value;
        pedroReferral.Accept();
        pedroReferral.RegisterFirstAttendance();
        referrals.Add(pedroReferral);

        // 2. NASF → CRASF (Ana Clara) — pendente: bloqueado pela falta do
        // consentimento de Assistência Social (caso 4, PersonConsentSeeder
        // deliberadamente não concede este tipo para ela).
        var anaClaraReferral = Referral.Create(
            persons["anaClara"].Id, units["NASF"].Id, units["CRASF"].Id,
            "Encaminhamento para avaliação socioassistencial da família", QueuePriority.ShortTerm,
            now.AddDays(-3)).Value;
        referrals.Add(anaClaraReferral);

        // 3. NAPE → CMA (Lucas Gabriel) — aceito, com fila resultante no CMA.
        var lucasReferral = Referral.Create(
            persons["lucas"].Id, units["NAPE"].Id, units["CMA"].Id,
            "Encaminhamento para avaliação fonoaudiológica", QueuePriority.ShortTerm, now.AddDays(-7)).Value;
        lucasReferral.Accept();
        referrals.Add(lucasReferral);

        resultingQueueEntries.Add(
            QueueEntry.Create(persons["lucas"].Id, units["CMA"].Id, "Fonoaudiologia", QueuePriority.ShortTerm, now.AddDays(-6)).Value);

        return (referrals, resultingQueueEntries);
    }
}
