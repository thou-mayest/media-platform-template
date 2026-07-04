using SharedKernal.Messaging;

namespace Users.Domain.DomainEvents;

public sealed record UserCreatedDomainEvent(
    Guid UserId,
    string Name,
    string Email) : IDomainEvent;
