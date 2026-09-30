using Users.Contracts.IntegrationEvents;
using Users.Domain.DomainEvents;

namespace Users.Application.Users.EventHandlers;

internal static class DomainEventToIntegrationMapper
{
    public static UserCreatedIntegrationEvent ToIntegrationEvent(
        this UserCreatedDomainEvent domainEvent)
        => new(
            domainEvent.UserId,
            domainEvent.Name,
            domainEvent.Email);

    public static UserDeletedIntegrationEvent ToIntegrationEvent(
    this UserDeletedDomainEvent domainEvent)
    => new(
        domainEvent.UserId);

    public static UserUpdatedIntegrationEvent ToIntegrationEvent(
        this UserUpdatedDomainEvent domainEvent)
        => new(
            domainEvent.UserId,
            domainEvent.Name,
            domainEvent.Email);
}
