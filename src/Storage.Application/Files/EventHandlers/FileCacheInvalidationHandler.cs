using Microsoft.Extensions.Caching.Hybrid;
using SharedKernal.Messaging;
using Storage.Domain.DomainEvents;

namespace Storage.Application.Files.EventHandlers
{
    internal sealed class FileCacheInvalidationHandler(HybridCache cache) :
        IDomainEventHandler<MediaAssetCreatedDomainEvent>,
        IDomainEventHandler<MediaAssetDeletedDomainEvent>,
        IDomainEventHandler<MediaAssetUpdateDomainEvent>

    {
        public Task Handle(MediaAssetCreatedDomainEvent notification, CancellationToken cancellationToken) => InvalidateByTag(cancellationToken);

        public Task Handle(MediaAssetDeletedDomainEvent notification, CancellationToken cancellationToken) => InvalidateByTag(cancellationToken);

        public  Task Handle(MediaAssetUpdateDomainEvent notification, CancellationToken cancellationToken) => InvalidateByTag(cancellationToken);

        private async Task InvalidateByTag(CancellationToken cancellationToken)
        {
            await cache.RemoveByTagAsync(FilesCacheKeys.Tag, cancellationToken);
        }
    }
}
