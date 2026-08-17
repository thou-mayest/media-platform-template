using MassTransit;
using Profiles.Contracts.IntegrationEvents;
using Profiles.Domain.DomainEvents;
using SharedKernal.Messaging;

namespace Profiles.Application.ActorProfiles.EventHandlers;

internal sealed class ActorProfileDeletedDomainEventHandler(IPublishEndpoint publishEndpoint)
    : IDomainEventHandler<ActorProfileDeletedDomainEvent>
{
    public Task Handle(
        ActorProfileDeletedDomainEvent notification, CancellationToken cancellationToken) =>
        publishEndpoint.Publish(
            new ActorProfileDeletedIntegrationEvent(notification.ProfileId),
            cancellationToken);
}
