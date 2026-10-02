namespace Profiles.Contracts.IntegrationEvents;

/// <summary>
/// Carries only the fields other modules denormalise. Albums stores actorSlug
/// and actorName on each album so a card renders without a second lookup, and
/// those copies go stale unless this event keeps them current.
///
/// Bio, profession and avatar are deliberately absent: nobody denormalises
/// them, and a contract carrying fields no consumer reads is a contract that
/// breaks consumers whenever an unrelated field changes.
/// </summary>
public sealed record ActorProfileUpdatedIntegrationEvent(
    Guid ProfileId,
    string Slug,
    string DisplayName);
