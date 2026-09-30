using SharedKernal.Messaging.DomainEvents;
using SharedKernal.Messaging.Outbox;
using System.Text.Json;
using Users.Application.Abstractions;

namespace Users.Infrastracture.Persistence
{
    internal class OutboxRepository(UsersDbContext context) : IOutboxRepository
    {
        public async Task AddAsync(IIntegrationEvent integrationEvent)
        {

            OutboxMessage message = new OutboxMessage
            {
                Id = Guid.NewGuid(),
                OccurredOnUtc = DateTime.UtcNow,
                Type = integrationEvent.GetType().FullName!,
                Content = JsonSerializer.Serialize(integrationEvent, integrationEvent.GetType())
            };

            await context.Set<OutboxMessage>().AddAsync(message);
        }
    }
}
