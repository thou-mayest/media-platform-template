using Profiles.Domain;
using SharedKernal.Pagination;

namespace Profiles.Application.Abstractions;

internal interface IActorProfileRepository
{
    Task<ActorProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<ActorProfile?> GetPublishedBySlugAsync(string slug, CancellationToken cancellationToken = default);

    Task<ActorProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    
    Task<bool> SlugExistsAsync(string slug, CancellationToken cancellationToken = default);


    Task<PagedResult<ActorProfile>> GetIndexableAsync(
        PageRequest request, CancellationToken cancellationToken = default);

    Task AddAsync(ActorProfile profile, CancellationToken cancellationToken = default);

    void Update(ActorProfile profile);

    void Remove(ActorProfile profile);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
