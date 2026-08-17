using MassTransit;
using Profiles.Contracts.IntegrationEvents;
using Profiles.Domain.DomainEvents;
using SharedKernal.Messaging;

namespace Profiles.Application.ActorProfiles.EventHandlers;


internal sealed class ActorProfilePublishedDomainEventHandler(IPublishEndpoint publishEndpoint)
    : IDomainEventHandler<ActorProfilePublishedDomainEvent>
{
    public Task Handle(
        ActorProfilePublishedDomainEvent notification, CancellationToken cancellationToken) =>
        publishEndpoint.Publish(
            new ActorProfilePublishedIntegrationEvent(
                notification.ProfileId,
                notification.UserId,
                notification.Slug,
                notification.DisplayName),
            cancellationToken);
}
