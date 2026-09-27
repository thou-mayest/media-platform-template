using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SharedKernal.Messaging.Outbox;
using System.Text.Json;
public sealed class OutboxProcessorBackgroundService<TDbContext, TIntegrationEventAssembly>(
        IServiceScopeFactory scopeFactory,
        ILogger<OutboxProcessorBackgroundService<TDbContext, TIntegrationEventAssembly>> logger) : BackgroundService where TDbContext : DbContext
{
    private readonly TimeSpan _pollInterval = TimeSpan.FromSeconds(5);
    private const int BatchSize = 20;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(_pollInterval);

        while (!stoppingToken.IsCancellationRequested &&
               await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await ProcessOutboxMessagesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while processing the outbox queue.");
            }
        }
    }

    private async Task ProcessOutboxMessagesAsync(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TDbContext>();
        var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

        var messages = await context.Set<OutboxMessage>()
            .Where(m => m.ProcessedOnUtc == null)
            .OrderBy(m => m.OccurredOnUtc)
            .Take(BatchSize)
            .ToListAsync(cancellationToken);

        if (messages.Count == 0) return;

        foreach (var message in messages)
        {
            try
            {
                var eventType = typeof(TIntegrationEventAssembly).Assembly.GetType(message.Type);
                var alltypes = typeof(TIntegrationEventAssembly).Assembly.GetTypes();
                if (eventType is null)
                {
                    throw new InvalidOperationException($"Could not resolve type: {message.Type}");
                }

                var domainEvent = JsonSerializer.Deserialize(message.Content, eventType);
                if (domainEvent is null)
                {
                    throw new InvalidOperationException($"Failed to deserialize outbox message {message.Id}");
                }

                await publishEndpoint.Publish(domainEvent, eventType, cancellationToken);

                message.ProcessedOnUtc = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to process outbox message {MessageId}", message.Id);
                message.Error = ex.Message;
            }
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}