using SharedKernal.Messaging;

namespace Storage.Domain.DomainEvents;

public sealed record MediaAssetUpdateDomainEvent(Guid guid) : IDomainEvent;