namespace Sigis.Domain.Abstractions;

/// <summary>
/// Classe base para entidades do domínio, identificadas por um identificador
/// único e não por seus atributos.
/// </summary>
public abstract class Entity
{
    /// <summary>Identificador único da entidade.</summary>
    public Guid Id { get; protected init; }
}
