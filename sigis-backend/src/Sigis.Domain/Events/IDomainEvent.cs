namespace Sigis.Domain.Events;

/// <summary>
/// Representa um evento de domínio ocorrido no sistema, acumulado por um
/// agregado raiz e publicado posteriormente pela camada de aplicação.
/// </summary>
public interface IDomainEvent
{
    /// <summary>Identificador único da ocorrência do evento.</summary>
    Guid Id { get; }

    /// <summary>Momento (UTC) em que o evento ocorreu.</summary>
    DateTime OccurredAt { get; }
}
