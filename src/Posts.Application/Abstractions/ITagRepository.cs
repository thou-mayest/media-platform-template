using Posts.Domain;

namespace Posts.Application.Abstractions;

internal interface ITagRepository
{
    /// <summary>
    /// Returns tracked tags for the given candidates, inserting any that don't exist yet.
    /// Matching is by <see cref="Tag.NormalizedName"/>; an existing tag keeps its original display name.
    /// </summary>
    Task<IReadOnlyList<Tag>> GetOrCreateAsync(IReadOnlyCollection<Tag> candidates, CancellationToken cancellationToken);
}
