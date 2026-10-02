using SharedKernal.Messaging.DomainEvents;

namespace Storage.Contracts.IntegrationEvents;

public sealed record FileUploadedIntegrationEvent(
    Guid FileId) : IIntegrationEvent;
