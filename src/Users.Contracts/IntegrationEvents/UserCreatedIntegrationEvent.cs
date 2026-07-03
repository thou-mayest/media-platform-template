namespace Users.Contracts.IntegrationEvents;

public sealed record UserCreatedIntegrationEvent(
    Guid UserId,
    string Name,
    string Email);
