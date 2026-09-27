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
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
           CancellationToken cancellationToken = default)
    {
        DbContext? dbContext = eventData.Context;

        if (dbContext is null)
        {
            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        var aggregates = dbContext.ChangeTracker
            .Entries<AggregateRoot>()
            .Where(e => e.Entity.DomainEvents.Count > 0)
            .Select(e => e.Entity)
            .ToList();

        var domainEvents = aggregates
            .SelectMany(a => a.DomainEvents)
            .ToList();

        if (domainEvents.Count == 0)
            return await base.SavingChangesAsync(eventData, result, cancellationToken);

        aggregates.ForEach(e => e.ClearDomainEvents());

        // dispatch domain events
        var dispatcherService = dbContext.GetService<IDomainEventDispatcher>();
        if (dispatcherService is not null)
        {
            await dispatcherService.DispatchAsync(domainEvents, cancellationToken);
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
