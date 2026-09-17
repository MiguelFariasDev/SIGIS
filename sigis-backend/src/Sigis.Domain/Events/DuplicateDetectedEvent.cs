namespace Sigis.Domain.Events;

/// <summary>Evento disparado quando um alerta de possível duplicidade de cadastro é registrado.</summary>
/// <param name="PersonId1">Identificador do primeiro cadastro envolvido no alerta.</param>
/// <param name="PersonId2">Identificador do segundo cadastro envolvido no alerta.</param>
/// <param name="SimilarityScore">Score de similaridade calculado entre os dois cadastros (0 a 1).</param>
public sealed record DuplicateDetectedEvent(Guid PersonId1, Guid PersonId2, double SimilarityScore) : IDomainEvent
{
    /// <inheritdoc />
    public Guid Id { get; } = Guid.NewGuid();

    /// <inheritdoc />
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
