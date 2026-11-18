using SharedKernal.Messaging.DomainEvents;
using SharedKernal.Messaging.Outbox;
using Storage.Domain.DomainEvents;

namespace Storage.Application.Files.EventHandlers;

internal class DomainEventToOutboxHundler(IOutboxRepository outboxRepository) :
        IDomainEventHandler<MediaAssetCreatedDomainEvent>

{
    public Task Handle(MediaAssetCreatedDomainEvent notification, CancellationToken cancellationToken) => outboxRepository.AddAsync(notification.ToIntegrationEvent());
}

internal class MediaAssetUploadCompletedToOutboxHandler(IOutboxRepository outboxRepository) :
        IDomainEventHandler<MediaAssetUploadCompletedDomainEvent>
{
    public Task Handle(MediaAssetUploadCompletedDomainEvent notification, CancellationToken cancellationToken) =>
        outboxRepository.AddAsync(notification.ToIntegrationEvent());
}

internal class MediaAssetUploadFailedToOutboxHandler(IOutboxRepository outboxRepository) :
        IDomainEventHandler<MediaAssetUploadFailedDomainEvent>
{
    public Task Handle(MediaAssetUploadFailedDomainEvent notification, CancellationToken cancellationToken) =>
        outboxRepository.AddAsync(notification.ToIntegrationEvent());
}
