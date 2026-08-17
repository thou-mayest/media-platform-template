using MassTransit;
using Profiles.Contracts.IntegrationEvents;
using Profiles.Domain.DomainEvents;
using SharedKernal.Messaging;

namespace Profiles.Application.ActorProfiles.EventHandlers;


internal sealed class ActorProfileUpdatedDomainEventHandler(IPublishEndpoint publishEndpoint)
    : IDomainEventHandler<ActorProfileUpdatedDomainEvent>
{
    public Task Handle(
        ActorProfileUpdatedDomainEvent notification, CancellationToken cancellationToken) =>
        publishEndpoint.Publish(
            new ActorProfileUpdatedIntegrationEvent(
                notification.ProfileId,
                notification.Slug,
                notification.DisplayName),
            cancellationToken);
}
