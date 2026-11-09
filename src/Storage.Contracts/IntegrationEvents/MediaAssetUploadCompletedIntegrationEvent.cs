using SharedKernal.Messaging.DomainEvents;

namespace Storage.Contracts.IntegrationEvents;

public sealed record MediaAssetUploadCompletedIntegrationEvent(
    Guid MediaAssetId) : IIntegrationEvent;
