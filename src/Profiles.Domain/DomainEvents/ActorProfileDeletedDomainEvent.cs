using SharedKernal.Messaging;

namespace Profiles.Domain.DomainEvents;

public sealed record ActorProfileDeletedDomainEvent(Guid ProfileId) : IDomainEvent;