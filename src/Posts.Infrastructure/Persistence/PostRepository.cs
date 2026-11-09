using Microsoft.EntityFrameworkCore;
using Posts.Application.Abstractions;
using Posts.Application.Posts;
using Posts.Domain;

namespace Posts.Infrastructure.Persistence;

internal sealed class PostRepository(PostsDbContext context) : IPostRepository
{
    public async Task AddAsync(Post post, CancellationToken cancellationToken)
    {
        await context.Posts.AddAsync(post, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        context.SaveChangesAsync(cancellationToken);

    public async Task<PagedPostsDto> GetPublishedAsync(
        string? keyword,
        string? category,
        string? tag,
        PostSort sort,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = context.Posts
            .AsNoTracking()
            .Where(post => post.Status == PostStatus.Published);

        if (keyword is not null)
        {
            var pattern = $"%{EscapeLikePattern(keyword)}%";
            query = query.Where(post =>
                EF.Functions.ILike(post.Title, pattern, "\\") ||
                EF.Functions.ILike(post.Description, pattern, "\\") ||
                EF.Functions.ILike(post.Category!.Name, pattern, "\\") ||
                post.Tags.Any(value => EF.Functions.ILike(value.Name, pattern, "\\")));
        }

        if (category is not null)
        {
            var normalizedCategory = Category.Normalize(category);
            query = query.Where(post => post.Category!.NormalizedName == normalizedCategory);
        }

        if (tag is not null)
        {
            var normalizedTag = PostTag.Normalize(tag);
            query = query.Where(post => post.Tags.Any(value => value.NormalizedName == normalizedTag));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        query = sort switch
        {
            PostSort.Popular => query.OrderByDescending(post => post.ViewCount)
                .ThenByDescending(post => post.PublishedAt)
                .ThenBy(post => post.Id),
            PostSort.Oldest => query.OrderBy(post => post.PublishedAt).ThenBy(post => post.Id),
            PostSort.Title => query.OrderBy(post => post.Title).ThenBy(post => post.Id),
            _ => query.OrderByDescending(post => post.PublishedAt).ThenBy(post => post.Id)
        };

        var items = await Project(query)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedPostsDto(
            items,
            page,
            pageSize,
            totalCount,
            totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize));
    }

    public Task<PostDto?> GetPublishedByIdAsync(Guid id, CancellationToken cancellationToken) =>
        Project(context.Posts.AsNoTracking().Where(post =>
                post.Id == id && post.Status == PostStatus.Published))
            .SingleOrDefaultAsync(cancellationToken);

    public async Task<PostFacetsDto> GetPublishedFacetsAsync(CancellationToken cancellationToken)
    {
        var categoryRows = await context.Posts
            .AsNoTracking()
            .Where(post => post.Status == PostStatus.Published)
            .GroupBy(post => new { post.Category!.NormalizedName, post.Category.Name })
            .Select(group => new
            {
                group.Key.NormalizedName,
                Name = group.Key.Name,
                Count = group.Count()
            })
            .ToListAsync(cancellationToken);

        var categories = categoryRows
            .Select(row => new PostFacetDto(row.Name, row.Count))
            .OrderByDescending(facet => facet.Count)
            .ThenBy(facet => facet.Name)
            .ToList();

        var tagRows = await context.Posts
            .AsNoTracking()
            .Where(post => post.Status == PostStatus.Published)
            .SelectMany(post => post.Tags)
            .GroupBy(tag => new { tag.NormalizedName, tag.Name })
            .Select(group => new
            {
                group.Key.NormalizedName,
                Name = group.Key.Name,
                Count = group.Count()
            })
            .ToListAsync(cancellationToken);

        var tags = tagRows
            .GroupBy(row => row.NormalizedName)
            .Select(group => new PostFacetDto(
                group.OrderByDescending(row => row.Count).ThenBy(row => row.Name).First().Name,
                group.Sum(row => row.Count)))
            .OrderByDescending(facet => facet.Count)
            .ThenBy(facet => facet.Name)
            .ToList();

        return new PostFacetsDto(categories, tags);
    }

    public async Task<bool> IncrementPublishedViewCountAsync(Guid id, CancellationToken cancellationToken)
    {
        var updated = await context.Posts
            .Where(post => post.Id == id && post.Status == PostStatus.Published)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(post => post.ViewCount, post => post.ViewCount + 1),
                cancellationToken);
        return updated == 1;
    }

    private static IQueryable<PostDto> Project(IQueryable<Post> query) =>
        query.Select(post => new PostDto(
            post.Id,
            post.AuthorId,
            post.MediaAssetId,
            post.Title,
            post.Description,
            post.MediaUrl,
            post.AltText,
            post.Category!.Name,
            post.Tags.OrderBy(tag => tag.Name).Select(tag => tag.Name).ToList(),
            post.Status.ToString(),
            post.CreatedDate,
            post.UpdateDate,
            post.PublishedAt,
            post.ViewCount));

    private static string EscapeLikePattern(string value) =>
        value.Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("%", "\\%", StringComparison.Ordinal)
            .Replace("_", "\\_", StringComparison.Ordinal);
}
