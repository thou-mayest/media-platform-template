namespace Users.Contracts.IntegrationEvents;

public sealed record UserUpdatedIntegrationEvent(
    Guid UserId,
    string Name,
    string Email);
