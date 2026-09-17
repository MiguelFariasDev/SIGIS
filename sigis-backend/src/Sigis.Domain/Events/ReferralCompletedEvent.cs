namespace Sigis.Domain.Events;

/// <summary>Evento disparado quando um encaminhamento é concluído após o primeiro atendimento na unidade de destino.</summary>
/// <param name="ReferralId">Identificador do encaminhamento concluído.</param>
/// <param name="PersonId">Identificador da pessoa encaminhada.</param>
public sealed record ReferralCompletedEvent(Guid ReferralId, Guid PersonId) : IDomainEvent
{
    /// <inheritdoc />
    public Guid Id { get; } = Guid.NewGuid();

    /// <inheritdoc />
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
