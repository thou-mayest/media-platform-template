using SharedKernal.Messaging.DomainEvents;

namespace Users.Application.Abstractions;

internal interface IOutboxRepository
{
    Task AddAsync(IIntegrationEvent integrationEvent);
}
