using SharedKernal.Messaging;

namespace Profiles.Domain.DomainEvents;

public sealed record ActorProfilePublishedDomainEvent(
    Guid ProfileId,
    Guid UserId,
    string Slug,
    string DisplayName) : IDomainEvent;