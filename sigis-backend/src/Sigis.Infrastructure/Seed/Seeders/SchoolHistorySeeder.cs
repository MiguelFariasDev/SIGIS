using Sigis.Domain.Entities;

namespace Sigis.Infrastructure.Seed.Seeders;

/// <summary>Semeia histórico escolar (RF-EDU) para algumas das pessoas de demonstração — não todas.</summary>
public static class SchoolHistorySeeder
{
    /// <summary>
    /// Cria históricos escolares para 4 das 12 pessoas: as gêmeas de
    /// cadastro (caso 1/2 — cadastradas via NAPE) e dois perfis comuns em
    /// idade escolar.
    /// </summary>
    /// <param name="persons">Pessoas já semeadas, indexadas pela chave curta do seed.</param>
    /// <param name="professionals">Profissionais já semeados, indexados pela chave curta do seed.</param>
    /// <param name="now">Instante de referência (UTC) de criação dos registros.</param>
    /// <returns>Os históricos escolares criados.</returns>
    public static List<SchoolHistory> Seed(
        IReadOnlyDictionary<string, Person> persons, IReadOnlyDictionary<string, Professional> professionals, DateTime now)
    {
        var currentYear = now.Year;
        var createdBy = professionals["psicologo"].Id;

        var histories = new List<SchoolHistory>();

        void Add(string personKey, string schoolName, string grade, string shift, string classGroup, string? notes = null)
        {
            var result = SchoolHistory.Create(
                persons[personKey].Id, schoolName, grade, currentYear, createdBy, now, currentYear,
                shift: shift, classGroup: classGroup, notes: notes);
            histories.Add(result.Value);
        }

        Add("twin1", "EMEIF Professora Maria do Carmo", "1º ano", "Manhã", "A",
            notes: "Encaminhada pela escola para avaliação de suporte pedagógico (TEA).");
        Add("twin2", "EMEIF Professora Maria do Carmo", "1º ano", "Manhã", "A",
            notes: "Encaminhada pela escola para avaliação de suporte pedagógico (TEA).");
        Add("joaoPedro", "EMEIF São Francisco de Assis", "3º ano", "Tarde", "B");
        Add("mariaEduarda", "Escola Municipal Vinte e Cinco de Março", "2º ano", "Manhã", "C",
            notes: "Acompanhamento pedagógico especializado em curso.");

        return histories;
    }
}
