using MassTransit;
using SharedKernal.Messaging;
using Users.Contracts.IntegrationEvents;
using Users.Domain.DomainEvents;

namespace Users.Application.Users.EventHandlers;

internal sealed class UserDeletedDomainEventHandler(IPublishEndpoint publishEndpoint)
    : IDomainEventHandler<UserDeletedDomainEvent>
{
    public Task Handle(UserDeletedDomainEvent notification, CancellationToken cancellationToken) =>
        publishEndpoint.Publish(
            new UserDeletedIntegrationEvent(notification.UserId),
            cancellationToken);
}
