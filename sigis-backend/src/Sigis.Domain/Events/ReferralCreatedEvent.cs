namespace Sigis.Domain.Events;

/// <summary>Evento disparado quando um encaminhamento de pessoa entre unidades é criado.</summary>
/// <param name="ReferralId">Identificador do encaminhamento.</param>
/// <param name="PersonId">Identificador da pessoa encaminhada.</param>
/// <param name="OriginUnitId">Identificador da unidade de origem.</param>
/// <param name="DestinationUnitId">Identificador da unidade de destino.</param>
public sealed record ReferralCreatedEvent(Guid ReferralId, Guid PersonId, Guid OriginUnitId, Guid DestinationUnitId)
    : IDomainEvent
{
    /// <inheritdoc />
    public Guid Id { get; } = Guid.NewGuid();

    /// <inheritdoc />
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
