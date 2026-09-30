using SharedKernal.Messaging.DomainEvents;

namespace Storage.Domain.DomainEvents;

public sealed record MediaAssetDeletedDomainEvent(Guid guid) : IDomainEvent;
