using SharedKernal.Messaging;

namespace Storage.Domain.DomainEvents;

public sealed record MediaAssetCreatedDomainEvent(Guid Guid) : IDomainEvent;