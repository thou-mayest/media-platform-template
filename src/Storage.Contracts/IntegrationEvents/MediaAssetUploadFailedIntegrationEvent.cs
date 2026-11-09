using SharedKernal.Messaging.DomainEvents;

namespace Storage.Contracts.IntegrationEvents;

public sealed record MediaAssetUploadFailedIntegrationEvent(
    Guid MediaAssetId) : IIntegrationEvent;
