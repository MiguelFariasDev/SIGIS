using Sigis.Domain.Enums;

namespace Sigis.Infrastructure.Seed.SeedData;

/// <summary>Dado estático de um profissional a ser semeado.</summary>
/// <param name="Key">Chave curta de referência interna do seed (ex.: "coordenador").</param>
/// <param name="Name">Nome completo.</param>
/// <param name="Specialty">Especialidade/cargo.</param>
/// <param name="Email">E-mail de login.</param>
/// <param name="Role">Papel RBAC.</param>
/// <param name="UnitAcronym">Sigla da unidade à qual está vinculado (ver <see cref="ServiceUnitSeedData"/>).</param>
public sealed record ProfessionalSeed(
    string Key, string Name, string Specialty, string Email, RbacRole Role, string UnitAcronym);

/// <summary>Os 5 profissionais de demonstração — senha "Senha123!" para todos.</summary>
public static class ProfessionalSeedData
{
    /// <summary>Senha padrão de todos os profissionais de demonstração.</summary>
    public const string DemoPassword = "Senha123!";

    /// <summary>Profissionais a semear.</summary>
    public static readonly IReadOnlyList<ProfessionalSeed> Professionals =
    [
        new("coordenador", "Camila Rodrigues", "Coordenadora de Rede", "coordenador@sigis.gov.br", RbacRole.Coordinator, "NASF"),
        new("psicologo", "João Pedro Silva", "Psicólogo", "psicologo@sigis.gov.br", RbacRole.Professional, "NAPE"),
        new("fono", "Maria Fernanda Costa", "Fonoaudióloga", "fono@cma.gov.br", RbacRole.Professional, "CMA"),
        new("assistente", "Carlos Eduardo Lima", "Assistente Social", "assistente@crasf.gov.br", RbacRole.Professional, "CRASF"),
        new("auditor", "Ana Beatriz Souza", "Auditora/DPO", "auditor@sigis.gov.br", RbacRole.Auditor, "NASF"),
    ];
}
