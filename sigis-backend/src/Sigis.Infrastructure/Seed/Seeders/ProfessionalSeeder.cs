using Sigis.Domain.Entities;
using Sigis.Domain.Interfaces;
using Sigis.Domain.ValueObjects;
using Sigis.Infrastructure.Seed.SeedData;

namespace Sigis.Infrastructure.Seed.Seeders;

/// <summary>Semeia os profissionais de demonstração.</summary>
public static class ProfessionalSeeder
{
    /// <summary>Cria os profissionais de <see cref="ProfessionalSeedData.Professionals"/>.</summary>
    /// <param name="units">Unidades de serviço já semeadas, indexadas por sigla.</param>
    /// <param name="passwordHasher">Serviço de hash de senha usado para gerar a senha padrão de demonstração.</param>
    /// <returns>Os profissionais criados, indexados pela chave curta do seed.</returns>
    public static Dictionary<string, Professional> Seed(
        IReadOnlyDictionary<string, ServiceUnit> units, IPasswordHasher passwordHasher)
    {
        var passwordHash = passwordHasher.Hash(ProfessionalSeedData.DemoPassword);
        var professionals = new Dictionary<string, Professional>();

        foreach (var seed in ProfessionalSeedData.Professionals)
        {
            var professional = Professional.Create(
                PersonName.Create(seed.Name).Value,
                seed.Specialty,
                units[seed.UnitAcronym].Id,
                seed.Role,
                EmailAddress.Create(seed.Email).Value,
                passwordHash).Value;

            professionals[seed.Key] = professional;
        }

        return professionals;
    }
}
