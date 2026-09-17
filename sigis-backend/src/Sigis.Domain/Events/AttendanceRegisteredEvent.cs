namespace Sigis.Domain.Events;

/// <summary>Evento disparado quando o comparecimento de uma pessoa a um atendimento é registrado.</summary>
/// <param name="AttendanceId">Identificador do atendimento.</param>
/// <param name="PersonId">Identificador da pessoa atendida.</param>
/// <param name="UnitId">Identificador da unidade de serviço onde ocorreu o atendimento.</param>
public sealed record AttendanceRegisteredEvent(Guid AttendanceId, Guid PersonId, Guid UnitId) : IDomainEvent
{
    /// <inheritdoc />
    public Guid Id { get; } = Guid.NewGuid();

    /// <inheritdoc />
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
