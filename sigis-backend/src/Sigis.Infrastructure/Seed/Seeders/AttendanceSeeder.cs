using Sigis.Domain.Entities;
using Sigis.Domain.Enums;

namespace Sigis.Infrastructure.Seed.Seeders;

/// <summary>Semeia os atendimentos (~15 no total, cobrindo os 5 tipos de sessão).</summary>
public static class AttendanceSeeder
{
    /// <summary>
    /// Cria os atendimentos de demonstração: o histórico de Pedro Henrique
    /// (caso 3 — timeline federada em 3 unidades) e as 2 faltas consecutivas
    /// de Lucas Gabriel (caso 5 — busca ativa), mais 10 atendimentos
    /// adicionais distribuídos entre as demais pessoas para cobrir os 5
    /// tipos de sessão com dados realistas.
    /// </summary>
    /// <param name="persons">Pessoas já semeadas, indexadas pela chave curta do seed.</param>
    /// <param name="units">Unidades de serviço já semeadas, indexadas por sigla.</param>
    /// <param name="professionals">Profissionais já semeados, indexados pela chave curta do seed.</param>
    /// <param name="now">Instante de referência (UTC) de criação dos registros.</param>
    /// <returns>Os atendimentos criados.</returns>
    public static List<Attendance> Seed(
        IReadOnlyDictionary<string, Person> persons,
        IReadOnlyDictionary<string, ServiceUnit> units,
        IReadOnlyDictionary<string, Professional> professionals,
        DateTime now)
    {
        var attendances = new List<Attendance>();

        // Caso 3 — Pedro Henrique: histórico em 3 unidades (timeline federada).
        var pedroNasf = Attendance.Create(
            persons["pedro"].Id, units["NASF"].Id, professionals["coordenador"].Id,
            new DateTime(2024, 3, 10, 9, 0, 0, DateTimeKind.Utc), SessionType.NasfRecord, sessionNumber: 1,
            createdAt: new DateTime(2024, 3, 10, 9, 0, 0, DateTimeKind.Utc),
            formData: """{"hipoteseDiagnostica":"Investigação de TEA","cid":"F84"}""",
            triagedByProfessionalId: professionals["coordenador"].Id,
            mainComplaint: "Atraso na fala e dificuldade de contato visual").Value;
        pedroNasf.MarkAsAttended();
        attendances.Add(pedroNasf);

        var pedroNape = Attendance.Create(
            persons["pedro"].Id, units["NAPE"].Id, professionals["psicologo"].Id,
            new DateTime(2024, 6, 20, 14, 0, 0, DateTimeKind.Utc), SessionType.PsychologicalAnamnesis, sessionNumber: 1,
            createdAt: new DateTime(2024, 6, 20, 14, 0, 0, DateTimeKind.Utc),
            formData: """{"observacoesComportamentais":"Dificuldade de interação social em grupo"}""").Value;
        pedroNape.MarkAsAttended();
        attendances.Add(pedroNape);

        var pedroCrasf = Attendance.Create(
            persons["pedro"].Id, units["CRASF"].Id, professionals["assistente"].Id,
            new DateTime(2024, 9, 5, 10, 0, 0, DateTimeKind.Utc), SessionType.Synthesis, sessionNumber: 1,
            createdAt: new DateTime(2024, 9, 5, 10, 0, 0, DateTimeKind.Utc),
            formData: """{"visitaSocial":"Família com boa rede de apoio, acompanhamento contínuo recomendado"}""").Value;
        pedroCrasf.MarkAsAttended();
        attendances.Add(pedroCrasf);

        // Caso 5 — Lucas Gabriel: 2 faltas consecutivas (RN04).
        var lucasFalta1 = Attendance.Create(
            persons["lucas"].Id, units["NAPE"].Id, professionals["psicologo"].Id,
            new DateTime(2024, 10, 15, 9, 0, 0, DateTimeKind.Utc), SessionType.PsychologicalAnamnesis, sessionNumber: 1,
            createdAt: new DateTime(2024, 10, 15, 9, 0, 0, DateTimeKind.Utc)).Value;
        lucasFalta1.MarkAsAbsent();
        attendances.Add(lucasFalta1);

        var lucasFalta2 = Attendance.Create(
            persons["lucas"].Id, units["NAPE"].Id, professionals["psicologo"].Id,
            new DateTime(2024, 11, 15, 9, 0, 0, DateTimeKind.Utc), SessionType.PsychologicalAnamnesis, sessionNumber: 2,
            createdAt: new DateTime(2024, 11, 15, 9, 0, 0, DateTimeKind.Utc)).Value;
        lucasFalta2.MarkAsAbsent();
        attendances.Add(lucasFalta2);

        // Demais pessoas — cobrem os tipos de sessão restantes com dados realistas.
        var twin1 = Attendance.Create(
            persons["twin1"].Id, units["NASF"].Id, professionals["coordenador"].Id, now.AddDays(-30),
            SessionType.NasfRecord, sessionNumber: 1, createdAt: now.AddDays(-30),
            formData: """{"hipoteseDiagnostica":"Em investigação"}""",
            mainComplaint: "Encaminhada pela creche por atraso no desenvolvimento").Value;
        twin1.MarkAsAttended();
        attendances.Add(twin1);

        var twin2 = Attendance.Create(
            persons["twin2"].Id, units["NAPE"].Id, professionals["psicologo"].Id, now.AddDays(-28),
            SessionType.PsychologicalAnamnesis, sessionNumber: 1, createdAt: now.AddDays(-28),
            formData: """{"observacoesComportamentais":"Boa adaptação inicial ao ambiente escolar"}""").Value;
        twin2.MarkAsAttended();
        attendances.Add(twin2);

        var anaClara = Attendance.Create(
            persons["anaClara"].Id, units["NASF"].Id, professionals["coordenador"].Id, now.AddDays(-45),
            SessionType.NasfRecord, sessionNumber: 1, createdAt: now.AddDays(-45),
            formData: """{"hipoteseDiagnostica":"TEA confirmado"}""",
            mainComplaint: "Atraso na comunicação verbal relatado pela família").Value;
        anaClara.MarkAsAttended();
        attendances.Add(anaClara);

        var joaoPedro = Attendance.Create(
            persons["joaoPedro"].Id, units["NASF"].Id, professionals["coordenador"].Id, now.AddDays(2),
            SessionType.NasfRecord, sessionNumber: 1, createdAt: now.AddDays(-2)).Value;
        attendances.Add(joaoPedro);

        var mariaEduarda = Attendance.Create(
            persons["mariaEduarda"].Id, units["NASF"].Id, professionals["coordenador"].Id, now.AddDays(-60),
            SessionType.NasfRecord, sessionNumber: 1, createdAt: now.AddDays(-60),
            formData: """{"hipoteseDiagnostica":"TEA confirmado"}""").Value;
        mariaEduarda.MarkAsAttended();
        attendances.Add(mariaEduarda);

        var luizGustavo = Attendance.Create(
            persons["luizGustavo"].Id, units["NAPE"].Id, professionals["psicologo"].Id, now.AddDays(-20),
            SessionType.PsychopedagogicalAnamnesis, sessionNumber: 1, createdAt: now.AddDays(-20),
            formData: """{"disciplinasComDificuldade":["Matemática","Português"]}""",
            triagedByProfessionalId: professionals["psicologo"].Id,
            mainComplaint: "Dificuldade de atenção e concentração em sala de aula").Value;
        luizGustavo.MarkAsAttended();
        attendances.Add(luizGustavo);

        var isabelly = Attendance.Create(
            persons["isabelly"].Id, units["NAPE"].Id, professionals["psicologo"].Id, now.AddDays(-40),
            SessionType.PhysicalEducationInstrument, sessionNumber: 1, createdAt: now.AddDays(-40),
            formData: """{"coordenacaoMotora":"Comprometimento leve","atendimentosConcomitantes":["Terapia Ocupacional"]}""").Value;
        isabelly.MarkAsAttended();
        attendances.Add(isabelly);

        var sofiaVitoria = Attendance.Create(
            persons["sofiaVitoria"].Id, units["NASF"].Id, professionals["coordenador"].Id, now.AddDays(5),
            SessionType.NasfRecord, sessionNumber: 1, createdAt: now.AddDays(-10)).Value;
        attendances.Add(sofiaVitoria);

        var rafael = Attendance.Create(
            persons["rafael"].Id, units["NASF"].Id, professionals["coordenador"].Id, now.AddMonths(-4),
            SessionType.NasfRecord, sessionNumber: 3, createdAt: now.AddMonths(-4),
            formData: """{"hipoteseDiagnostica":"TEA confirmado","observacoes":"Acompanhamento de continuidade"}""",
            mainComplaint: "Retorno de acompanhamento de rotina").Value;
        rafael.MarkAsAttended();
        attendances.Add(rafael);

        var gabriel = Attendance.Create(
            persons["gabriel"].Id, units["NASF"].Id, professionals["coordenador"].Id, now.AddDays(10),
            SessionType.NasfRecord, sessionNumber: 1, createdAt: now.AddMonths(-6)).Value;
        attendances.Add(gabriel);

        return attendances;
    }
}
