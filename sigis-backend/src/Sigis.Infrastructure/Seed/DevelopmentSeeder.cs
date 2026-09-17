using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sigis.Domain.Entities;
using Sigis.Domain.Interfaces;
using Sigis.Infrastructure.Persistence.Context;
using Sigis.Infrastructure.Seed.Seeders;

namespace Sigis.Infrastructure.Seed;

/// <summary>
/// Orquestra o seed de desenvolvimento do SIGIS: unidades, profissionais,
/// pessoas (incluindo os 5 casos de demonstração do pitch — gêmeas de
/// cadastro, timeline federada, consentimento parcial, busca ativa e
/// encaminhamento aceito), filas, atendimentos, encaminhamentos, alerta de
/// duplicidade, consentimentos LGPD e logs de auditoria. Não deve ser
/// executado em produção — só é chamado quando <c>IWebHostEnvironment.IsDevelopment()</c>
/// é verdadeiro (ver <c>Program.cs</c>).
/// </summary>
public static class DevelopmentSeeder
{
    /// <summary>
    /// Executa o seed de desenvolvimento, caso o banco ainda esteja vazio
    /// (idempotente: se já houver unidades de serviço cadastradas, não faz
    /// nada).
    /// </summary>
    /// <param name="context">Contexto de persistência do SIGIS.</param>
    /// <param name="passwordHasher">Serviço de hash de senha usado para os profissionais de demonstração.</param>
    /// <param name="logger">Logger para acompanhar o progresso do seed — nunca registra CNS/CPF/senha em texto claro.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    public static async Task SeedAsync(
        SigisDbContext context, IPasswordHasher passwordHasher, ILogger logger, CancellationToken cancellationToken)
    {
        if (await context.ServiceUnits.AnyAsync(cancellationToken))
        {
            logger.LogInformation("Seed de desenvolvimento já aplicado — nada a fazer.");
            return;
        }

        var now = DateTime.UtcNow;
        var today = DateOnly.FromDateTime(now);

        logger.LogInformation("Seed: unidades de serviço...");
        var units = ServiceUnitSeeder.Seed();
        await context.ServiceUnits.AddRangeAsync(units.Values, cancellationToken);

        logger.LogInformation("Seed: profissionais...");
        var professionals = ProfessionalSeeder.Seed(units, passwordHasher);
        await context.Professionals.AddRangeAsync(professionals.Values, cancellationToken);

        logger.LogInformation("Seed: pessoas (5 casos de demonstração + 7 perfis comuns)...");
        var persons = PersonSeeder.Seed(today);
        await context.Persons.AddRangeAsync(persons.Values, cancellationToken);

        logger.LogInformation("Seed: filas de atendimento...");
        var queueEntries = QueueEntrySeeder.Seed(persons, units, now);
        await context.QueueEntries.AddRangeAsync(queueEntries, cancellationToken);

        logger.LogInformation("Seed: atendimentos...");
        var attendances = AttendanceSeeder.Seed(persons, units, professionals, now);
        await context.Attendances.AddRangeAsync(attendances, cancellationToken);

        logger.LogInformation("Seed: encaminhamentos...");
        var (referrals, referralQueueEntries) = ReferralSeeder.Seed(persons, units, now);
        await context.Referrals.AddRangeAsync(referrals, cancellationToken);
        await context.QueueEntries.AddRangeAsync(referralQueueEntries, cancellationToken);

        logger.LogInformation("Seed: alerta de duplicidade (gêmeas de cadastro)...");
        var duplicateAlert = DuplicateAlertSeeder.Seed(persons, now);
        await context.DuplicateAlerts.AddAsync(duplicateAlert, cancellationToken);

        logger.LogInformation("Seed: consentimentos LGPD...");
        var consents = PersonConsentSeeder.Seed(persons, now);
        await context.PersonConsents.AddRangeAsync(consents, cancellationToken);

        logger.LogInformation("Seed: logs de auditoria de acesso...");
        var accessLogs = AccessLogSeeder.Seed(persons, professionals, now);
        await context.AccessLogs.AddRangeAsync(accessLogs, cancellationToken);

        // Cobertura clínica (perfil clínico + atendimentos concomitantes) —
        // algumas pessoas, não todas: Pedro Henrique (caso 3), Luiz Gustavo
        // (TDAH) e Rafael (adulto em continuidade de cuidado).
        logger.LogInformation("Seed: perfis clínicos e atendimentos concomitantes...");
        var (clinicalProfiles, concurrentTreatments) = ClinicalCoverageSeeder.Seed(persons, units, now);
        await context.PersonClinicalProfiles.AddRangeAsync(clinicalProfiles, cancellationToken);
        await context.ConcurrentTreatments.AddRangeAsync(concurrentTreatments, cancellationToken);

        // Histórico escolar — 4 das 12 pessoas.
        logger.LogInformation("Seed: histórico escolar...");
        var schoolHistories = SchoolHistorySeeder.Seed(persons, professionals, now);
        await context.SchoolHistories.AddRangeAsync(schoolHistories, cancellationToken);

        // Composição familiar — 3 das 12 pessoas.
        logger.LogInformation("Seed: composição familiar...");
        var familyCompositions = FamilyCompositionSeeder.Seed(persons, now);
        await context.FamilyCompositions.AddRangeAsync(familyCompositions, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Seed de desenvolvimento concluído: {UnitCount} unidades, {ProfessionalCount} profissionais, " +
            "{PersonCount} pessoas, {QueueCount} filas, {AttendanceCount} atendimentos, {ReferralCount} " +
            "encaminhamentos, 1 alerta de duplicidade, {ConsentCount} consentimentos, {AccessLogCount} logs de " +
            "acesso, {ClinicalProfileCount} perfis clínicos, {ConcurrentTreatmentCount} atendimentos " +
            "concomitantes, {SchoolHistoryCount} históricos escolares, {FamilyCompositionCount} composições " +
            "familiares.",
            units.Count, professionals.Count, persons.Count, queueEntries.Count + referralQueueEntries.Count,
            attendances.Count, referrals.Count, consents.Count, accessLogs.Count, clinicalProfiles.Count,
            concurrentTreatments.Count, schoolHistories.Count, familyCompositions.Count);
    }
}
