using Sigis.Domain.Entities;
using Sigis.Domain.ValueObjects;
using Sigis.Infrastructure.Seed.SeedData;

namespace Sigis.Infrastructure.Seed.Seeders;

/// <summary>Semeia as pessoas de demonstração.</summary>
public static class PersonSeeder
{
    /// <summary>Cria as pessoas de <see cref="PersonSeedData.Persons"/>.</summary>
    /// <param name="today">Data de referência (UTC) para validação de data de nascimento.</param>
    /// <returns>As pessoas criadas, indexadas pela chave curta do seed.</returns>
    public static Dictionary<string, Person> Seed(DateOnly today)
    {
        var persons = new Dictionary<string, Person>();

        foreach (var seed in PersonSeedData.Persons)
        {
            var address = seed.AddressStreet is null
                ? null
                : Address.Create(
                    seed.AddressStreet, seed.AddressNumber, seed.AddressNeighborhood,
                    seed.AddressCity, seed.AddressState, seed.AddressZip).Value;

            var person = Person.Create(
                PersonName.Create(seed.Name).Value,
                seed.BirthDate,
                today,
                cns: seed.CnsDigits is null ? null : Cns.Create(seed.CnsDigits).Value,
                cpf: seed.CpfDigits is null ? null : Cpf.Create(seed.CpfDigits).Value,
                motherName: seed.MotherName,
                address: address).Value;

            if (seed.DisabilityTypes is not null)
            {
                person.UpdateEducationalProfile(
                    naturality: null, currentSchool: null, grade: null, shift: null, classGroup: null, zone: null,
                    schoolEnrollment: null, referredBySchool: null, disabilityTypes: seed.DisabilityTypes,
                    needsSpecialEducation: null, attendsTutoring: null, hasFailedGrade: null);
            }

            persons[seed.Key] = person;
        }

        return persons;
    }
}
