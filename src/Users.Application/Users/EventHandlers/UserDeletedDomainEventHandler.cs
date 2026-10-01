using SharedKernal.Messaging.DomainEvents;
using SharedKernal.Messaging.Outbox;
using Users.Domain.DomainEvents;

namespace Users.Application.Users.EventHandlers;

internal sealed class UserDeletedDomainEventHandler(IOutboxRepository outbox)
    : IDomainEventHandler<UserDeletedDomainEvent>
{
    public Task Handle(UserDeletedDomainEvent notification, CancellationToken cancellationToken) =>
        outbox.AddAsync(notification.ToIntegrationEvent());
}
