using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using SharedKernal.Entities;
using SharedKernal.Messaging.DomainEvents;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;

namespace SharedKernal.Messaging.Outbox;

public sealed class DomainEventsInterceptor
    : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        DbContext? dbContext = eventData.Context;

        if (dbContext is null)
        {
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        var aggregates = dbContext.ChangeTracker
            .Entries<AggregateRoot>()
            .Where(e => e.Entity.DomainEvents.Count > 0)
            .Select(e => e.Entity)
            .ToList();

        var domainEvents = aggregates
            .SelectMany(a => a.DomainEvents)
            .ToList();

        aggregates.ForEach(e => e.ClearDomainEvents());

        

        var outboxMessages = domainEvents.Select(domainEvent => new OutboxMessage
            {
                Id = Guid.NewGuid(),
                OccurredOnUtc = DateTime.UtcNow,
                Type = domainEvent.GetType().Name,
                Content = JsonSerializer.Serialize(domainEvent, domainEvent.GetType())
            })
            .ToList();

        // TODO: handle each event seperatly or MT transactional outbox
        
        // save integration events in outbox
        if (outboxMessages.Any())
        {
            dbContext.Set<OutboxMessage>().AddRange(outboxMessages);
        }

        // dispatch domain events
        var dispatcherService = dbContext.GetService<IDomainEventDispatcher>();
        if (dispatcherService is not null)
        {
            dispatcherService.DispatchAsync(domainEvents, cancellationToken);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
