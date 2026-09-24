using SharedKernal.Messaging;

namespace Storage.Domain.DomainEvents;

public sealed record MediaAssetDeletedDomainEvent(Guid guid) : IDomainEvent;
