using Sigis.Domain.Entities;
using Sigis.Infrastructure.Seed.SeedData;

namespace Sigis.Infrastructure.Seed.Seeders;

/// <summary>Semeia as unidades de serviço da rede municipal.</summary>
public static class ServiceUnitSeeder
{
    /// <summary>Cria as unidades de <see cref="ServiceUnitSeedData.Units"/>.</summary>
    /// <returns>As unidades criadas, indexadas por sigla.</returns>
    public static Dictionary<string, ServiceUnit> Seed()
    {
        var units = new Dictionary<string, ServiceUnit>();

        foreach (var seed in ServiceUnitSeedData.Units)
        {
            var unit = ServiceUnit.Create(seed.Name, seed.Acronym, seed.Secretariat).Value;
            units[seed.Acronym] = unit;
        }

        return units;
    }
}
