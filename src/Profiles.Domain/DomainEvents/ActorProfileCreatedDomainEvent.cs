using SharedKernal.Messaging;

namespace Profiles.Domain.DomainEvents;

/// <summary>Raised on draft creation. Stays inside the module — no integration
/// event, because a draft nobody can see is not news to other modules.</summary>
public sealed record ActorProfileCreatedDomainEvent(
    Guid ProfileId,
    Guid UserId,
    string Slug) : IDomainEvent;