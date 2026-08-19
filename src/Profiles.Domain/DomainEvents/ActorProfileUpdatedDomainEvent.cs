using SharedKernal.Messaging;

namespace Profiles.Domain.DomainEvents;

public sealed record ActorProfileUpdatedDomainEvent(
    Guid ProfileId,
    string Slug,
    string DisplayName) : IDomainEvent;