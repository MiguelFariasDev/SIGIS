using Sigis.Domain.Abstractions;
using Sigis.Domain.Abstractions.Errors;
using Sigis.Domain.ValueObjects;

namespace Sigis.Domain.Entities;

/// <summary>
/// Responsável legal por uma pessoa cadastrada (geralmente um dos pais ou
/// tutor de uma criança/adolescente com TEA).
/// </summary>
public sealed class Guardian : Entity
{
    /// <summary>Identificador da pessoa da qual este é responsável.</summary>
    public Guid PersonId { get; private set; }

    /// <summary>Nome completo do responsável.</summary>
    public PersonName Name { get; private set; }

    /// <summary>Número do CNS do responsável, quando informado.</summary>
    public Cns? Cns { get; private set; }

    /// <summary>Data de nascimento do responsável, quando informada.</summary>
    public DateOnly? BirthDate { get; private set; }

    /// <summary>Grau de parentesco com a pessoa (ex.: "Mãe", "Pai", "Tutor legal").</summary>
    public string Relationship { get; private set; }

    private Guardian(Guid id, Guid personId, PersonName name, Cns? cns, DateOnly? birthDate, string relationship)
    {
        Id = id;
        PersonId = personId;
        Name = name;
        Cns = cns;
        BirthDate = birthDate;
        Relationship = relationship;
    }

    /// <summary>
    /// Cria um novo <see cref="Guardian"/> vinculado a uma pessoa.
    /// </summary>
    /// <param name="personId">Identificador da pessoa da qual este é responsável.</param>
    /// <param name="name">Nome completo do responsável, já validado como <see cref="PersonName"/>.</param>
    /// <param name="relationship">Grau de parentesco com a pessoa.</param>
    /// <param name="cns">Número do CNS do responsável, opcional.</param>
    /// <param name="birthDate">Data de nascimento do responsável, opcional.</param>
    /// <returns>
    /// Um <see cref="Result{T}"/> de sucesso, ou de falha com
    /// <see cref="PersonErrors.RelacionamentoObrigatorio"/> quando o
    /// relacionamento não for informado.
    /// </returns>
    public static Result<Guardian> Create(
        Guid personId,
        PersonName name,
        string? relationship,
        Cns? cns = null,
        DateOnly? birthDate = null)
    {
        if (string.IsNullOrWhiteSpace(relationship))
            return Result<Guardian>.Failure(PersonErrors.RelacionamentoObrigatorio);

        return Result<Guardian>.Success(
            new Guardian(Guid.NewGuid(), personId, name, cns, birthDate, relationship.Trim()));
    }

    /// <summary>
    /// Atualiza os dados cadastrais do responsável.
    /// </summary>
    /// <param name="name">Novo nome completo.</param>
    /// <param name="relationship">Novo grau de parentesco.</param>
    /// <param name="cns">Novo CNS, opcional.</param>
    /// <param name="birthDate">Nova data de nascimento, opcional.</param>
    /// <returns>
    /// Um <see cref="Result"/> de sucesso, ou de falha com
    /// <see cref="PersonErrors.RelacionamentoObrigatorio"/> quando o
    /// relacionamento não for informado.
    /// </returns>
    public Result UpdateData(PersonName name, string? relationship, Cns? cns, DateOnly? birthDate)
    {
        if (string.IsNullOrWhiteSpace(relationship))
            return Result.Failure(PersonErrors.RelacionamentoObrigatorio);

        Name = name;
        Relationship = relationship.Trim();
        Cns = cns;
        BirthDate = birthDate;
        return Result.Success();
    }
}
