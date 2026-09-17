using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Events;

namespace Sigis.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Interceptor de <see cref="DbContext.SaveChanges"/> que, após a
/// persistência bem-sucedida, publica os eventos de domínio acumulados por
/// cada <see cref="AggregateRoot"/> rastreado e limpa a lista — garantindo
/// que eventos só sejam publicados depois que a transação realmente
/// ocorreu, evitando notificar sobre algo que acabou não sendo salvo.
/// </summary>
public sealed class DomainEventDispatcherInterceptor : SaveChangesInterceptor
{
    private readonly IPublisher _publisher;

    /// <summary>Cria o interceptor de despacho de eventos de domínio.</summary>
    /// <param name="publisher">Publicador MediatR usado para propagar os eventos.</param>
    public DomainEventDispatcherInterceptor(IPublisher publisher)
    {
        _publisher = publisher;
    }

    /// <inheritdoc />
    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
            await DispatchEventsAsync(eventData.Context, cancellationToken);

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    private async Task DispatchEventsAsync(DbContext context, CancellationToken cancellationToken)
    {
        var aggregatesWithEvents = context.ChangeTracker.Entries<AggregateRoot>()
            .Select(entry => entry.Entity)
            .Where(aggregate => aggregate.Events.Count > 0)
            .ToList();

        foreach (var aggregate in aggregatesWithEvents)
        {
            var events = aggregate.Events.ToList();
            aggregate.ClearEvents();

            foreach (var domainEvent in events)
                await PublishAsync(domainEvent, cancellationToken);
        }
    }

    private Task PublishAsync(IDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var notificationType = typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType());
        var notification = (INotification)Activator.CreateInstance(notificationType, domainEvent)!;
        return _publisher.Publish(notification, cancellationToken);
    }
}
