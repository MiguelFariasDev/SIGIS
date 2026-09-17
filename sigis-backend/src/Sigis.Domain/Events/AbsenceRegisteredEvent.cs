namespace Sigis.Domain.Events;

/// <summary>Evento disparado quando a falta de uma pessoa a um atendimento é registrada.</summary>
/// <param name="AttendanceId">Identificador do atendimento.</param>
/// <param name="PersonId">Identificador da pessoa que faltou.</param>
/// <param name="UnitId">Identificador da unidade de serviço onde o atendimento estava marcado.</param>
public sealed record AbsenceRegisteredEvent(Guid AttendanceId, Guid PersonId, Guid UnitId) : IDomainEvent
{
    /// <inheritdoc />
    public Guid Id { get; } = Guid.NewGuid();

    /// <inheritdoc />
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
