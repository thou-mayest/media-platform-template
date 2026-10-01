using SharedKernal.Messaging.DomainEvents;
using SharedKernal.Messaging.Outbox;
using Users.Domain.DomainEvents;

namespace Users.Application.Users.EventHandlers;

internal sealed class UserCreatedDomainEventHandler(IOutboxRepository outbox)
    : IDomainEventHandler<UserCreatedDomainEvent>
{
    public Task Handle(UserCreatedDomainEvent notification, CancellationToken cancellationToken) =>
        outbox.AddAsync(notification.ToIntegrationEvent());
}
