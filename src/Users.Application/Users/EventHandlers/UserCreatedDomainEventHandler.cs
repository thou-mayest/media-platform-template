using MassTransit;
using SharedKernal.Messaging;
using Users.Contracts.IntegrationEvents;
using Users.Domain.DomainEvents;

namespace Users.Application.Users.EventHandlers;

internal sealed class UserCreatedDomainEventHandler(IPublishEndpoint publishEndpoint)
    : IDomainEventHandler<UserCreatedDomainEvent>
{
    public Task Handle(UserCreatedDomainEvent notification, CancellationToken cancellationToken) =>
        publishEndpoint.Publish(
            new UserCreatedIntegrationEvent(notification.UserId, notification.Name, notification.Email),
            cancellationToken);
}
