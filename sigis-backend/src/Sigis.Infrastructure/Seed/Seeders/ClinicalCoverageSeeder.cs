using Sigis.Domain.Entities;
using DayOfWeek = Sigis.Domain.Enums.DayOfWeek;

namespace Sigis.Infrastructure.Seed.Seeders;

/// <summary>
/// Semeia perfil clínico (RF-Saúde) e atendimentos concomitantes para
/// algumas das pessoas de demonstração — não todas.
/// </summary>
public static class ClinicalCoverageSeeder
{
    /// <summary>
    /// Cria perfis clínicos para 3 pessoas (Pedro Henrique — caso 3, Luiz
    /// Gustavo — TDAH, Rafael — adulto em continuidade de cuidado) e
    /// atendimentos concomitantes para 2 delas (Pedro Henrique, Rafael).
    /// </summary>
    /// <param name="persons">Pessoas já semeadas, indexadas pela chave curta do seed.</param>
    /// <param name="units">Unidades já semeadas, indexadas pela sigla.</param>
    /// <param name="now">Instante de referência (UTC) de criação dos registros.</param>
    /// <returns>Os perfis clínicos e atendimentos concomitantes criados.</returns>
    public static (List<PersonClinicalProfile> Profiles, List<ConcurrentTreatment> Treatments) Seed(
        IReadOnlyDictionary<string, Person> persons, IReadOnlyDictionary<string, ServiceUnit> units, DateTime now)
    {
        var profiles = new List<PersonClinicalProfile>
        {
            PersonClinicalProfile.Create(
                persons["pedro"].Id, now.AddMonths(-6), medicalRecordNumber: "NASF-0001",
                clinicalHypothesis: "TEA confirmado — CID F84", apsReferenceUnitId: units["NASF"].Id).Value,

            PersonClinicalProfile.Create(
                persons["luizGustavo"].Id, now.AddMonths(-4), medicalRecordNumber: "NASF-0002",
                clinicalHypothesis: "TDAH confirmado — CID F90.0", apsReferenceUnitId: units["NASF"].Id).Value,

            PersonClinicalProfile.Create(
                persons["rafael"].Id, now.AddYears(-2), medicalRecordNumber: "CMA-0001",
                clinicalHypothesis: "TEA confirmado — CID F84, continuidade de cuidado na vida adulta",
                apsReferenceUnitId: units["CMA"].Id).Value,
        };

        var treatments = new List<ConcurrentTreatment>
        {
            ConcurrentTreatment.Create(
                persons["pedro"].Id, "Terapeuta Ocupacional", "Casa Mais Azul", "Juliana Prado",
                DayOfWeek.Tuesday, new TimeOnly(14, 0), new TimeOnly(15, 0), now.AddMonths(-1)).Value,

            ConcurrentTreatment.Create(
                persons["rafael"].Id, "Psiquiatra", "Clínica particular", "Dr. Eduardo Farias",
                DayOfWeek.Thursday, new TimeOnly(9, 0), new TimeOnly(10, 0), now.AddMonths(-3),
                notes: "Acompanhamento medicamentoso — continuidade de cuidado.").Value,
        };

        return (profiles, treatments);
    }
}
