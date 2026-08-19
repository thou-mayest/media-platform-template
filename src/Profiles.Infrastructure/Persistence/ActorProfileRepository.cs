using Microsoft.EntityFrameworkCore;
using Profiles.Application.Abstractions;
using Profiles.Domain;
using SharedKernal.Messaging;
using SharedKernal.Pagination;

namespace Profiles.Infrastructure.Persistence;

internal class ActorProfileRepository(ProfilesDbContext context, IDomainEventDispatcher dispatcher)
    : IActorProfileRepository
{
    public async Task AddAsync(ActorProfile profile, CancellationToken cancellationToken = default)
    {
        await context.ActorProfiles.AddAsync(profile, cancellationToken);
    }

    public async Task<ActorProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.ActorProfiles.FindAsync([id], cancellationToken);
    }

    public async Task<ActorProfile?> GetPublishedBySlugAsync(
        string slug, CancellationToken cancellationToken = default)
    {
        return await context.ActorProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.IsPublished && p.Slug.Value == slug, cancellationToken);
    }

  
    /// <summary>
    /// Tracked. DeleteActorProfileByUserId mutates what this returns, and
    /// leaving it untracked would make the delete depend on Remove() attaching
    /// the instance before SaveChanges collects its domain events — correct
    /// today, and silently broken by any reordering. Tracking one row on a
    /// settings-page read costs nothing next to that.
    /// </summary>
    public async Task<ActorProfile?> GetByUserIdAsync(
        Guid userId, CancellationToken cancellationToken = default)
    {
        return await context.ActorProfiles
            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
    }

    public async Task<bool> SlugExistsAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await context.ActorProfiles
            .AnyAsync(p => p.Slug.Value == slug, cancellationToken);
    }

    public async Task<PagedResult<ActorProfile>> GetIndexableAsync(
        PageRequest request, CancellationToken cancellationToken = default)
    {
        var query = context.ActorProfiles
            .AsNoTracking()
            .Where(p => p.IsIndexable);

        var totalItems = await query.CountAsync(cancellationToken);

       
        var items = await query
            .OrderByDescending(p => p.CreatedDate)
            .ThenByDescending(p => p.Id)
            .Skip(request.Skip)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<ActorProfile>(items, request.Page, request.PageSize, totalItems);
    }

    public void Update(ActorProfile profile)
    {
        context.ActorProfiles.Update(profile);
    }

    public void Remove(ActorProfile profile)
    {
        context.ActorProfiles.Remove(profile);
    }


    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var aggregates = context.ChangeTracker
            .Entries<ActorProfile>()
            .Where(e => e.Entity.DomainEvents.Count > 0)
            .Select(e => e.Entity)
            .ToList();

        var domainEvents = aggregates
            .SelectMany(a => a.DomainEvents)
            .ToList();

        aggregates.ForEach(a => a.ClearDomainEvents());

        var result = await context.SaveChangesAsync(cancellationToken);

        await dispatcher.DispatchAsync(domainEvents, cancellationToken);

        return result;
    }
}
