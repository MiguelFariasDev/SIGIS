using Sigis.Domain.Abstractions;
using Sigis.Domain.Abstractions.Errors;
using Sigis.Domain.Enums;

namespace Sigis.Domain.Entities;

/// <summary>
/// Unidade de serviço da rede municipal responsável pelo atendimento a
/// pessoas com TEA (ex.: NASF, NAPE, CREAES, Casa Mais Azul, CRASF).
/// </summary>
public sealed class ServiceUnit : Entity
{
    /// <summary>Nome completo da unidade.</summary>
    public string Name { get; private set; }

    /// <summary>Sigla da unidade (ex.: "NASF", "NAPE").</summary>
    public string Acronym { get; private set; }

    /// <summary>Secretaria municipal responsável pela unidade.</summary>
    public ResponsibleSecretariat Secretariat { get; private set; }

    private ServiceUnit(Guid id, string name, string acronym, ResponsibleSecretariat secretariat)
    {
        Id = id;
        Name = name;
        Acronym = acronym;
        Secretariat = secretariat;
    }

    /// <summary>
    /// Cria uma nova <see cref="ServiceUnit"/>.
    /// </summary>
    /// <param name="name">Nome completo da unidade.</param>
    /// <param name="acronym">Sigla da unidade.</param>
    /// <param name="secretariat">Secretaria municipal responsável.</param>
    /// <returns>
    /// Um <see cref="Result{T}"/> de sucesso, ou de falha quando nome ou
    /// sigla não forem informados.
    /// </returns>
    public static Result<ServiceUnit> Create(string? name, string? acronym, ResponsibleSecretariat secretariat)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result<ServiceUnit>.Failure(PersonErrors.NomeUnidadeObrigatorio);

        if (string.IsNullOrWhiteSpace(acronym))
            return Result<ServiceUnit>.Failure(PersonErrors.SiglaUnidadeObrigatoria);

        return Result<ServiceUnit>.Success(
            new ServiceUnit(Guid.NewGuid(), name.Trim(), acronym.Trim().ToUpperInvariant(), secretariat));
    }

    /// <summary>Indica se a unidade pertence à Secretaria de Saúde.</summary>
    /// <returns><see langword="true"/> quando <see cref="Secretariat"/> é <see cref="ResponsibleSecretariat.Health"/>.</returns>
    public bool BelongsToHealth() => Secretariat == ResponsibleSecretariat.Health;

    /// <summary>Indica se a unidade pertence à Secretaria de Educação.</summary>
    /// <returns><see langword="true"/> quando <see cref="Secretariat"/> é <see cref="ResponsibleSecretariat.Education"/>.</returns>
    public bool BelongsToEducation() => Secretariat == ResponsibleSecretariat.Education;
}
