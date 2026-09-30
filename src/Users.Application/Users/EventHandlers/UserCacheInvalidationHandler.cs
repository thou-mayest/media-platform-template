using Microsoft.Extensions.Caching.Hybrid;
using SharedKernal.Messaging;
using Users.Domain.DomainEvents;

namespace Users.Application.Users.EventHandlers;

internal sealed class UserCacheInvalidationHandler(HybridCache cache)
    : IDomainEventHandler<UserCreatedDomainEvent>,
      IDomainEventHandler<UserUpdatedDomainEvent>,
      IDomainEventHandler<UserDeletedDomainEvent>
{
    public Task Handle(UserCreatedDomainEvent notification, CancellationToken cancellationToken) =>
        InvalidateAsync(cancellationToken);

    public Task Handle(UserUpdatedDomainEvent notification, CancellationToken cancellationToken) =>
        InvalidateAsync(cancellationToken);

    public Task Handle(UserDeletedDomainEvent notification, CancellationToken cancellationToken) =>
        InvalidateAsync(cancellationToken);

    private async Task InvalidateAsync(CancellationToken cancellationToken) =>
        await cache.RemoveByTagAsync(UserCacheKeys.Tag, cancellationToken);
}
