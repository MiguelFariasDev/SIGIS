using Microsoft.EntityFrameworkCore;
using Sigis.Domain.Entities;
using Sigis.Domain.Enums;
using Sigis.Domain.Interfaces;
using Sigis.Domain.ValueObjects;
using Sigis.Infrastructure.Persistence.Context;
using DayOfWeek = Sigis.Domain.Enums.DayOfWeek;

namespace Sigis.Infrastructure.Seed;

/// <summary>
/// Popula o banco de dados com dados de demonstração para uso em
/// desenvolvimento: unidades da rede, profissionais, pessoas (incluindo
/// casos propositais de possível duplicidade) e movimentação de fila,
/// atendimento e encaminhamento. Não deve ser executado em produção.
/// </summary>
public static class DevelopmentSeeder
{
    /// <summary>
    /// Executa o seed de desenvolvimento, caso o banco ainda esteja vazio.
    /// </summary>
    /// <param name="context">Contexto de persistência do SIGIS.</param>
    /// <param name="passwordHasher">Serviço de hash de senha usado para os profissionais de demonstração.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    public static async Task SeedAsync(
        SigisDbContext context, IPasswordHasher passwordHasher, CancellationToken cancellationToken)
    {
        if (await context.ServiceUnits.AnyAsync(cancellationToken))
            return;

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var now = DateTime.UtcNow;

        // 1. Unidades de serviço da rede municipal.
        var nasf = ServiceUnit.Create("Núcleo Ampliado de Saúde da Família", "NASF", ResponsibleSecretariat.Health).Value;
        var creaes = ServiceUnit.Create("Centro de Referência em Atenção Especializada em Saúde", "CREAES", ResponsibleSecretariat.Health).Value;
        var nape = ServiceUnit.Create("Núcleo de Atendimento Pedagógico Especializado", "NAPE", ResponsibleSecretariat.Education).Value;
        var casaMaisAzul = ServiceUnit.Create("Casa Mais Azul", "CASA_AZUL", ResponsibleSecretariat.Health).Value;
        var crasf = ServiceUnit.Create("Centro de Referência de Assistência Social e Familiar", "CRASF", ResponsibleSecretariat.SocialAssistance).Value;

        await context.ServiceUnits.AddRangeAsync([nasf, creaes, nape, casaMaisAzul, crasf], cancellationToken);

        // 2. Um profissional por papel RBAC — credenciais de demonstração
        // (senha "Senha123!" para todos, hash gerado via BCrypt).
        var demoPasswordHash = passwordHasher.Hash("Senha123!");

        var coordinator = Professional.Create(
            PersonName.Create("Carlos Eduardo Mendes").Value, "Coordenador de Rede", nasf.Id, RbacRole.Coordinator,
            EmailAddress.Create("coordenador@sigis.gov.br").Value, demoPasswordHash).Value;
        var professionalUser = Professional.Create(
            PersonName.Create("Ana Beatriz Ferreira").Value, "Psicóloga", nape.Id, RbacRole.Professional,
            EmailAddress.Create("psicologo@sigis.gov.br").Value, demoPasswordHash).Value;
        var auditor = Professional.Create(
            PersonName.Create("Fernanda Lima Rocha").Value, "Auditora/DPO", nasf.Id, RbacRole.Auditor,
            EmailAddress.Create("auditor@sigis.gov.br").Value, demoPasswordHash).Value;

        await context.Professionals.AddRangeAsync([professionalUser, coordinator, auditor], cancellationToken);

        // 3. Dez pessoas com nomes brasileiros comuns.
        var joaoPedro = Person.Create(
            PersonName.Create("João Pedro Alves Santos").Value, new DateOnly(2016, 3, 12), today,
            cns: Cns.Create("100000000010002").Value,
            phone: PhoneNumber.Create("(88) 99999-0001").Value).Value;

        var mariaEduarda = Person.Create(
            PersonName.Create("Maria Eduarda Costa Lima").Value, new DateOnly(2017, 7, 4), today,
            cns: Cns.Create("100000000020008").Value,
            email: EmailAddress.Create("responsavel.eduarda@example.com").Value).Value;

        var luizGustavo = Person.Create(
            PersonName.Create("Luiz Gustavo Pereira Souza").Value, new DateOnly(2014, 11, 30), today,
            cpf: Cpf.Create("52998224725").Value).Value;

        var anaClara = Person.Create(
            PersonName.Create("Ana Clara Rodrigues Melo").Value, new DateOnly(2018, 1, 20), today,
            cns: Cns.Create("100000000030003").Value).Value;

        var pedroHenrique = Person.Create(
            PersonName.Create("Pedro Henrique Oliveira Dias").Value, new DateOnly(2015, 5, 9), today,
            address: Address.Create("Rua das Flores", "45", "Centro", "Crateús", "CE", "63700-000").Value).Value;

        var sofiaVitoria = Person.Create(
            PersonName.Create("Sofia Vitória Barbosa Nunes").Value, new DateOnly(2019, 9, 2), today,
            cns: Cns.Create("100000000040009").Value).Value;

        var gabrielSilva = Person.Create(
            PersonName.Create("Gabriel da Silva Nascimento").Value, new DateOnly(2013, 4, 17), today).Value;

        var isabellyMoura = Person.Create(
            PersonName.Create("Isabelly Cristina Moura Rocha").Value, new DateOnly(2016, 12, 25), today,
            cns: Cns.Create("100000000050004").Value).Value;

        var rafaelAraujo = Person.Create(
            PersonName.Create("Rafael Augusto Araújo Lima").Value, new DateOnly(2012, 8, 8), today).Value;

        var laraBeatriz = Person.Create(
            PersonName.Create("Lara Beatriz Fernandes Cruz").Value, new DateOnly(2017, 2, 14), today,
            cns: Cns.Create("200000000010009").Value).Value;

        // 4. Duas pessoas "gêmeas" (mesmo nome e mesma data de nascimento) —
        // caso propositalmente ambíguo para a camada 3 de deduplicação
        // (RN02, matching probabilístico via pg_trgm).
        var twinBirthDate = new DateOnly(2015, 6, 21);
        var twinName = "Maria Clara Souza Lima";
        var twinOne = Person.Create(PersonName.Create(twinName).Value, twinBirthDate, today).Value;
        var twinTwo = Person.Create(PersonName.Create(twinName).Value, twinBirthDate, today).Value;

        // 5. Pessoa com histórico de atendimento em 3 unidades diferentes —
        // caso de uso da timeline consolidada (RF08).
        var multiUnitPerson = Person.Create(
            PersonName.Create("Beatriz Helena Castro Almeida").Value, new DateOnly(2014, 10, 3), today,
            cns: Cns.Create("200000000020004").Value).Value;

        await context.Persons.AddRangeAsync(
            [
                joaoPedro, mariaEduarda, luizGustavo, anaClara, pedroHenrique,
                sofiaVitoria, gabrielSilva, isabellyMoura, rafaelAraujo, laraBeatriz,
                twinOne, twinTwo, multiUnitPerson,
            ],
            cancellationToken);

        // 6. Filas variadas.
        var queueWaiting = QueueEntry.Create(
            joaoPedro.Id, nasf.Id, "Psicologia", QueuePriority.ShortTerm, now.AddDays(-2)).Value;

        var queueUrgent = QueueEntry.Create(
            mariaEduarda.Id, nape.Id, "Psicopedagogia", QueuePriority.Urgent, now.AddHours(-6)).Value;

        var queueInAttendance = QueueEntry.Create(
            anaClara.Id, creaes.Id, "Terapia Ocupacional", QueuePriority.WaitingList, now.AddDays(-10)).Value;
        queueInAttendance.CallForAttendance();

        var queueAbsent = QueueEntry.Create(
            gabrielSilva.Id, casaMaisAzul.Id, "Fonoaudiologia", QueuePriority.ShortTerm, now.AddDays(-5)).Value;
        queueAbsent.RegisterAbsence();

        await context.QueueEntries.AddRangeAsync(
            [queueWaiting, queueUrgent, queueInAttendance, queueAbsent], cancellationToken);

        // 7. Histórico de atendimento em 3 unidades para a mesma pessoa.
        var attendanceNasf = Attendance.Create(
            multiUnitPerson.Id, nasf.Id, professionalUser.Id, now.AddMonths(-3),
            SessionType.NasfRecord, sessionNumber: 1, createdAt: now.AddMonths(-3),
            formData: """{"hipoteseDiagnostica":"Em investigação"}""").Value;
        attendanceNasf.MarkAsAttended();

        var attendanceNape = Attendance.Create(
            multiUnitPerson.Id, nape.Id, professionalUser.Id, now.AddMonths(-2),
            SessionType.PsychopedagogicalAnamnesis, sessionNumber: 1, createdAt: now.AddMonths(-2),
            formData: """{"observacoes":"Boa evolução pedagógica"}""").Value;
        attendanceNape.MarkAsAttended();

        var attendanceCrasf = Attendance.Create(
            multiUnitPerson.Id, crasf.Id, coordinator.Id, now.AddDays(-15),
            SessionType.Synthesis, sessionNumber: 1, createdAt: now.AddDays(-15)).Value;

        await context.Attendances.AddRangeAsync([attendanceNasf, attendanceNape, attendanceCrasf], cancellationToken);

        // 8. Um encaminhamento pendente.
        var referral = Referral.Create(
            laraBeatriz.Id, nasf.Id, nape.Id, "Necessita avaliação psicopedagógica complementar",
            QueuePriority.ShortTerm, now.AddDays(-1)).Value;

        await context.Referrals.AddAsync(referral, cancellationToken);

        // 9. Um alerta de duplicidade pendente — as duas pessoas "gêmeas"
        // (mesmo nome e mesma data de nascimento) criadas no passo 4.
        var duplicateAlert = DuplicateAlert.Create(twinOne.Id, twinTwo.Id, 1.0, now.AddHours(-1)).Value;
        await context.DuplicateAlerts.AddAsync(duplicateAlert, cancellationToken);

        // 10. Perfil clínico NASF e um atendimento concomitante para a
        // pessoa com histórico multi-unidade — demonstra o ajuste de
        // cobertura (PersonClinicalProfile, ConcurrentTreatment).
        var clinicalProfile = PersonClinicalProfile.Create(
            multiUnitPerson.Id, now.AddMonths(-3), medicalRecordNumber: "NASF-0001",
            clinicalHypothesis: "Investigação de TEA", apsReferenceUnitId: nasf.Id).Value;
        await context.PersonClinicalProfiles.AddAsync(clinicalProfile, cancellationToken);

        var concurrentTreatment = ConcurrentTreatment.Create(
            multiUnitPerson.Id, "Terapeuta Ocupacional", "Casa Mais Azul", "Juliana Prado",
            DayOfWeek.Tuesday, new TimeOnly(14, 0), new TimeOnly(15, 0), now.AddMonths(-1)).Value;
        await context.ConcurrentTreatments.AddAsync(concurrentTreatment, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }
}
