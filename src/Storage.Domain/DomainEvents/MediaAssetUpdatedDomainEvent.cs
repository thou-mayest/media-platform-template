using SharedKernal.Messaging.DomainEvents;

namespace Storage.Domain.DomainEvents;

public sealed record MediaAssetUpdateDomainEvent(Guid guid) : IDomainEvent;