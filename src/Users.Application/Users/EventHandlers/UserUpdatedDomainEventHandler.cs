using SharedKernal.Messaging.DomainEvents;
using Users.Application.Abstractions;
using Users.Domain.DomainEvents;

namespace Users.Application.Users.EventHandlers;

internal sealed class UserUpdatedDomainEventHandler(IOutboxRepository outbox)
    : IDomainEventHandler<UserUpdatedDomainEvent>
{
    public Task Handle(UserUpdatedDomainEvent notification, CancellationToken cancellationToken) =>
        outbox.AddAsync(notification.ToIntegrationEvent());
}
