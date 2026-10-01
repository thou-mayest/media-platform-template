using Storage.Contracts.IntegrationEvents;
using Storage.Domain.DomainEvents;

namespace Storage.Application.Files.EventHandlers;

internal static class DomainEventToIntegrationMapper
{
    public static FileUploadedIntegrationEvent ToIntegrationEvent(
        this MediaAssetCreatedDomainEvent domainEvent)
        => new (domainEvent.Guid);
}

