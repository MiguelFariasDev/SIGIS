namespace Sigis.Domain.Events;

/// <summary>Evento disparado quando uma nova pessoa é registrada no cadastro único.</summary>
/// <param name="PersonId">Identificador da pessoa registrada.</param>
public sealed record PersonRegisteredEvent(Guid PersonId) : IDomainEvent
{
    /// <inheritdoc />
    public Guid Id { get; } = Guid.NewGuid();

    /// <inheritdoc />
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
