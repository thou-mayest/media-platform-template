namespace Profiles.Contracts.IntegrationEvents;

/// <summary>
/// Raised when a profile becomes publicly visible for the first time.
/// Creation is deliberately not published — profiles are provisioned as drafts
/// from UserCreatedIntegrationEvent, and a draft nobody can see is not news to
/// other modules.
/// </summary>
public sealed record ActorProfilePublishedIntegrationEvent(
    Guid ProfileId,
    Guid UserId,
    string Slug,
    string DisplayName);
