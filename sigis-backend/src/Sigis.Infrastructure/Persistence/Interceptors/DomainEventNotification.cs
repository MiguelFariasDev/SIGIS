using MediatR;
using Sigis.Domain.Events;

namespace Sigis.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Adapta um <see cref="IDomainEvent"/> puro do domínio para uma notificação
/// MediatR, permitindo publicá-lo via <see cref="IPublisher"/> sem que
/// <c>Sigis.Domain</c> precise depender do pacote MediatR — o domínio
/// permanece livre de dependências externas.
/// </summary>
/// <typeparam name="TDomainEvent">Tipo do evento de domínio encapsulado.</typeparam>
/// <param name="DomainEvent">Evento de domínio original, emitido por um agregado.</param>
public sealed record DomainEventNotification<TDomainEvent>(TDomainEvent DomainEvent) : INotification
    where TDomainEvent : IDomainEvent;
