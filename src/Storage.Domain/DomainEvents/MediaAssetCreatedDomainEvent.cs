using SharedKernal.Messaging.DomainEvents;

namespace Storage.Domain.DomainEvents;

public sealed record MediaAssetCreatedDomainEvent(Guid Guid) : IDomainEvent;