using SharedKernal.Messaging.DomainEvents;
using SharedKernal.Messaging.Outbox;
using Storage.Domain.DomainEvents;

namespace Storage.Application.Files.EventHandlers;

internal class DomainEventToOutboxHundler(IOutboxRepository outboxRepository) :
        IDomainEventHandler<MediaAssetCreatedDomainEvent>

{
    public Task Handle(MediaAssetCreatedDomainEvent notification, CancellationToken cancellationToken) => outboxRepository.AddAsync(notification.ToIntegrationEvent());
}
