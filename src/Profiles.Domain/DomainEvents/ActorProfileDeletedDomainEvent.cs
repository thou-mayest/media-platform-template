using SharedKernal.Messaging.DomainEvents;

namespace Profiles.Domain.DomainEvents;

public sealed record ActorProfileDeletedDomainEvent(Guid ProfileId) : IDomainEvent;