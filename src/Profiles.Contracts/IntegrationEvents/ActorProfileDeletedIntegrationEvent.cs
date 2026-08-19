namespace Profiles.Contracts.IntegrationEvents;

/// <summary>
/// Raised when a profile is removed. Consumers are expected to drop their
/// denormalised copies rather than leave albums pointing at a profile that no
/// longer resolves.
/// </summary>
public sealed record ActorProfileDeletedIntegrationEvent(Guid ProfileId);
