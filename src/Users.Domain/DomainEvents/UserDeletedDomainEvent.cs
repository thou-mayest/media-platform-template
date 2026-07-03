using SharedKernal.Messaging;
using SharedKernal.Messaging;

namespace Users.Domain.DomainEvents;

public sealed record UserDeletedDomainEvent(Guid UserId) : IDomainEvent;
