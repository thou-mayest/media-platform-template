using MassTransit;
using SharedKernal.Messaging.DomainEvents;
using Users.Application.Abstractions;
using Users.Contracts.IntegrationEvents;
using Users.Domain.DomainEvents;

namespace Users.Application.Users.EventHandlers;

internal sealed class UserDeletedDomainEventHandler(IOutboxRepository outbox)
    : IDomainEventHandler<UserDeletedDomainEvent>
{
    public Task Handle(UserDeletedDomainEvent notification, CancellationToken cancellationToken) =>
        outbox.AddAsync(notification.ToIntegrationEvent());
}
