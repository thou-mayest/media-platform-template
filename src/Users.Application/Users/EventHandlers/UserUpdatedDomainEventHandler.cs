using MassTransit;
using SharedKernal.Messaging;
using Users.Contracts.IntegrationEvents;
using Users.Domain.DomainEvents;

namespace Users.Application.Users.EventHandlers;

internal sealed class UserUpdatedDomainEventHandler(IPublishEndpoint publishEndpoint)
    : IDomainEventHandler<UserUpdatedDomainEvent>
{
    public Task Handle(UserUpdatedDomainEvent notification, CancellationToken cancellationToken) =>
        publishEndpoint.Publish(
            new UserUpdatedIntegrationEvent(notification.UserId, notification.Name, notification.Email),
            cancellationToken);
}
