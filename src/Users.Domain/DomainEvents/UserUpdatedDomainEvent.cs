using SharedKernal.Messaging;

namespace Users.Domain.DomainEvents;

public sealed record UserUpdatedDomainEvent(
    Guid UserId,
    string Name,
    string Email) : IDomainEvent;
