using Sigis.Domain.Entities;

namespace Sigis.Infrastructure.Seed.Seeders;

/// <summary>Semeia composição familiar (RF-EDU/Saúde) para algumas das pessoas de demonstração — não todas.</summary>
public static class FamilyCompositionSeeder
{
    /// <summary>
    /// Cria composições familiares para 3 das 12 pessoas: Ana Clara (caso 4
    /// — consentimento parcial) e dois perfis comuns.
    /// </summary>
    /// <param name="persons">Pessoas já semeadas, indexadas pela chave curta do seed.</param>
    /// <param name="now">Instante de referência (UTC) de criação dos registros.</param>
    /// <returns>As composições familiares criadas.</returns>
    public static List<FamilyComposition> Seed(IReadOnlyDictionary<string, Person> persons, DateTime now)
    {
        var compositions = new List<FamilyComposition>
        {
            FamilyComposition.Create(
                persons["anaClara"].Id, now,
                fatherName: "Roberto Pereira", fatherEducation: "Ensino Médio completo", fatherOccupation: "Motorista",
                motherName: "Fernanda Pereira", motherEducation: "Ensino Fundamental completo", motherOccupation: "Do lar",
                siblingsCount: 1, siblingsAges: "3 anos", householdMembersCount: 4,
                parentsMaritalStatus: "União estável", filiationType: "Natural",
                plannedPregnancy: true, pregnanciesCount: 2, abortionsCount: 0,
                deliveryType: "Cesarea").Value,

            FamilyComposition.Create(
                persons["sofiaVitoria"].Id, now,
                fatherName: "Marcos Nunes", motherName: "Juliana Barbosa",
                siblingsCount: 0, householdMembersCount: 3,
                parentsMaritalStatus: "Casados", filiationType: "Natural",
                plannedPregnancy: false, pregnanciesCount: 1, abortionsCount: 0,
                deliveryType: "Normal").Value,

            FamilyComposition.Create(
                persons["gabriel"].Id, now,
                motherName: "Cleide Nascimento", motherEducation: "Ensino Médio incompleto", motherOccupation: "Diarista",
                siblingsCount: 2, siblingsAges: "9 e 14 anos", householdMembersCount: 5,
                parentsMaritalStatus: "Mãe solo", filiationType: "Natural").Value,
        };

        return compositions;
    }
}
