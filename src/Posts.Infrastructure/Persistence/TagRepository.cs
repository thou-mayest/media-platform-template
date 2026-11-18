using Microsoft.EntityFrameworkCore;
using Posts.Application.Abstractions;
using Posts.Domain;

namespace Posts.Infrastructure.Persistence;

internal sealed class TagRepository(PostsDbContext context) : ITagRepository
{
    public async Task<IReadOnlyList<Tag>> GetOrCreateAsync(
        IReadOnlyCollection<Tag> candidates,
        CancellationToken cancellationToken)
    {
        if (candidates.Count == 0)
            return [];

        var unique = candidates
            .GroupBy(tag => tag.NormalizedName)
            .Select(group => group.First())
            .ToArray();

        var ids = unique.Select(tag => tag.Id).ToArray();
        var names = unique.Select(tag => tag.Name).ToArray();
        var normalizedNames = unique.Select(tag => tag.NormalizedName).ToArray();
        var versions = unique.Select(tag => tag.Version).ToArray();

        // ON CONFLICT makes concurrent creation of the same new tag safe: the loser simply reuses the winner's row.
        await context.Database.ExecuteSqlInterpolatedAsync($"""
            INSERT INTO "Posts"."Tags" ("Id", "Name", "NormalizedName", "UsageCount", "CreatedAt", "Version")
            SELECT t.id, t.name, t.normalized_name, 0, now(), t.version
            FROM unnest({ids}, {names}, {normalizedNames}, {versions}) AS t(id, name, normalized_name, version)
            ON CONFLICT ("NormalizedName") DO NOTHING
            """, cancellationToken);

        return await context.Tags
            .Where(tag => normalizedNames.Contains(tag.NormalizedName))
            .ToListAsync(cancellationToken);
    }
}
