using SharedKernal.Messaging.DomainEvents;

namespace SharedKernal.Messaging.Outbox;

public interface IOutboxRepository
{
    Task AddAsync(IIntegrationEvent integrationEvent);
}
