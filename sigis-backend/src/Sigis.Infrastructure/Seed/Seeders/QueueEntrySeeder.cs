using Sigis.Domain.Entities;
using Sigis.Domain.Enums;

namespace Sigis.Infrastructure.Seed.Seeders;

/// <summary>Semeia as entradas de fila de atendimento (12 no total, distribuídas entre 4 unidades).</summary>
public static class QueueEntrySeeder
{
    /// <summary>
    /// Cria as 12 entradas de fila da demonstração: 4 no NASF, 5 no NAPE, 2 no
    /// CMA e 1 no CRASF, com 2 em prioridade Urgente, 4 Curto Prazo e 6 Lista
    /// de Espera. A entrada de Lucas Gabriel (caso 5) recebe 2 faltas
    /// consecutivas, transicionando automaticamente para busca ativa (RN04).
    /// </summary>
    /// <param name="persons">Pessoas já semeadas, indexadas pela chave curta do seed.</param>
    /// <param name="units">Unidades de serviço já semeadas, indexadas por sigla.</param>
    /// <param name="now">Instante de referência (UTC) para calcular datas de entrada relativas.</param>
    /// <returns>As entradas de fila criadas.</returns>
    public static List<QueueEntry> Seed(
        IReadOnlyDictionary<string, Person> persons, IReadOnlyDictionary<string, ServiceUnit> units, DateTime now)
    {
        var entries = new List<QueueEntry>();

        // NASF (4) — 1 Urgente, 1 Curto Prazo, 2 Lista de Espera (uma delas de espera longa).
        entries.Add(QueueEntry.Create(persons["mariaEduarda"].Id, units["NASF"].Id, "Psicologia", QueuePriority.Urgent, now.AddHours(-6)).Value);
        entries.Add(QueueEntry.Create(persons["joaoPedro"].Id, units["NASF"].Id, "Terapia Ocupacional", QueuePriority.ShortTerm, now.AddDays(-2)).Value);
        entries.Add(QueueEntry.Create(persons["sofiaVitoria"].Id, units["NASF"].Id, "Fonoaudiologia", QueuePriority.WaitingList, now.AddDays(-10)).Value);
        entries.Add(QueueEntry.Create(persons["gabriel"].Id, units["NASF"].Id, "Psicologia", QueuePriority.WaitingList, now.AddMonths(-6)).Value);

        // NAPE (5) — 1 Urgente, 1 Curto Prazo (Lucas — vai para busca ativa), 3 Lista de Espera.
        entries.Add(QueueEntry.Create(persons["luizGustavo"].Id, units["NAPE"].Id, "Psicopedagogia", QueuePriority.Urgent, now.AddDays(-1)).Value);

        var lucasEntry = QueueEntry.Create(persons["lucas"].Id, units["NAPE"].Id, "Psicologia", QueuePriority.ShortTerm, now.AddMonths(-2)).Value;
        lucasEntry.RegisterAbsence();
        lucasEntry.RegisterAbsence();
        entries.Add(lucasEntry);

        entries.Add(QueueEntry.Create(persons["isabelly"].Id, units["NAPE"].Id, "Psicopedagogia", QueuePriority.WaitingList, now.AddDays(-15)).Value);
        entries.Add(QueueEntry.Create(persons["twin2"].Id, units["NAPE"].Id, "Psicopedagogia", QueuePriority.WaitingList, now.AddDays(-7)).Value);
        entries.Add(QueueEntry.Create(persons["rafael"].Id, units["NAPE"].Id, "Psicologia", QueuePriority.WaitingList, now.AddDays(-20)).Value);

        // CMA (2) — 1 Curto Prazo, 1 Lista de Espera.
        entries.Add(QueueEntry.Create(persons["pedro"].Id, units["CMA"].Id, "Fonoaudiologia", QueuePriority.ShortTerm, now.AddDays(-3)).Value);
        entries.Add(QueueEntry.Create(persons["twin1"].Id, units["CMA"].Id, "Fonoaudiologia", QueuePriority.WaitingList, now.AddDays(-12)).Value);

        // CRASF (1) — Curto Prazo.
        entries.Add(QueueEntry.Create(persons["anaClara"].Id, units["CRASF"].Id, "Assistência Social", QueuePriority.ShortTerm, now.AddDays(-4)).Value);

        return entries;
    }
}
