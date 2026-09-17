using Sigis.Domain.Events;

namespace Sigis.Domain.Abstractions;

/// <summary>
/// Classe base para agregados raiz do domínio, responsável por acumular
/// eventos de domínio ocorridos durante operações da entidade até que sejam
/// publicados pela camada de aplicação.
/// </summary>
public abstract class AggregateRoot : Entity
{
    private readonly List<IDomainEvent> _events = [];

    /// <summary>Eventos de domínio ocorridos e ainda não publicados.</summary>
    public IReadOnlyCollection<IDomainEvent> Events => _events.AsReadOnly();

    /// <summary>
    /// Registra um evento de domínio para publicação posterior pela infraestrutura.
    /// </summary>
    /// <param name="domainEvent">Evento ocorrido durante uma operação do agregado.</param>
    protected void Raise(IDomainEvent domainEvent) => _events.Add(domainEvent);

    /// <summary>
    /// Remove todos os eventos de domínio acumulados. Deve ser chamado pela
    /// infraestrutura de persistência após publicá-los (ex.: logo após
    /// <c>SaveChangesAsync</c>).
    /// </summary>
    public void ClearEvents() => _events.Clear();
}
