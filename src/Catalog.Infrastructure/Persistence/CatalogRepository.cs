using Catalog.Application.Abstractions;
using Catalog.Domain;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Persistence;

internal sealed class CatalogRepository(CatalogDbContext dbContext) : ICatalogRepository
{
    public async Task<IReadOnlyList<Creator>> GetCreatorsAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Creators
            .AsNoTracking()
            .AsSplitQuery()
            .Include(creator => creator.Videos)
                .ThenInclude(video => video.Category)
            .Include(creator => creator.Videos)
                .ThenInclude(video => video.VideoTags)
                    .ThenInclude(link => link.Tag)
            .OrderBy(creator => creator.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Category>> GetCategoriesAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Categories
            .AsNoTracking()
            .AsSplitQuery()
            .Include(category => category.Videos)
                .ThenInclude(video => video.Creator)
            .Include(category => category.Videos)
                .ThenInclude(video => video.VideoTags)
                    .ThenInclude(link => link.Tag)
            .OrderBy(category => category.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Tag>> GetTagsAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Tags
            .AsNoTracking()
            .Include(tag => tag.VideoTags)
            .OrderBy(tag => tag.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Video>> GetVideosAsync(CancellationToken cancellationToken)
    {
        return await VideosQuery()
            .OrderBy(video => video.DisplayOrder)
            .ToListAsync(cancellationToken);
    }

    public Task<Creator?> GetCreatorBySlugAsync(string slug, CancellationToken cancellationToken)
    {
        return dbContext.Creators
            .AsNoTracking()
            .AsSplitQuery()
            .Include(creator => creator.Videos)
                .ThenInclude(video => video.Category)
            .Include(creator => creator.Videos)
                .ThenInclude(video => video.VideoTags)
                    .ThenInclude(link => link.Tag)
            .FirstOrDefaultAsync(creator => creator.Slug == slug, cancellationToken);
    }

    public Task<Category?> GetCategoryBySlugAsync(string slug, CancellationToken cancellationToken)
    {
        return dbContext.Categories
            .AsNoTracking()
            .AsSplitQuery()
            .Include(category => category.Videos)
                .ThenInclude(video => video.Creator)
            .Include(category => category.Videos)
                .ThenInclude(video => video.VideoTags)
                    .ThenInclude(link => link.Tag)
            .FirstOrDefaultAsync(category => category.Slug == slug, cancellationToken);
    }

    public Task<Tag?> GetTagBySlugAsync(string slug, CancellationToken cancellationToken)
    {
        return dbContext.Tags
            .AsSplitQuery()
            .Include(tag => tag.VideoTags)
                .ThenInclude(link => link.Video)
                    .ThenInclude(video => video.Creator)
            .Include(tag => tag.VideoTags)
                .ThenInclude(link => link.Video)
                    .ThenInclude(video => video.Category)
            .Include(tag => tag.VideoTags)
                .ThenInclude(link => link.Video)
                    .ThenInclude(video => video.VideoTags)
                        .ThenInclude(innerLink => innerLink.Tag)
            .FirstOrDefaultAsync(tag => tag.Slug == slug, cancellationToken);
    }

    public Task<Video?> GetVideoBySlugAsync(string slug, CancellationToken cancellationToken)
    {
        return VideosQuery()
            .FirstOrDefaultAsync(video => video.Slug == slug, cancellationToken);
    }

    public Task<Category?> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.Categories
            .Include(category => category.Videos)
            .FirstOrDefaultAsync(category => category.Id == id, cancellationToken);
    }

    public Task<Category?> GetCategoryByNameAsync(string name, CancellationToken cancellationToken)
    {
        return dbContext.Categories
            .FirstOrDefaultAsync(category => category.Name.ToLower() == name.ToLower(), cancellationToken);
    }

    public Task<Category?> GetDefaultCategoryAsync(CancellationToken cancellationToken)
    {
        return dbContext.Categories
            .FirstOrDefaultAsync(category => category.Slug == Category.DefaultSlug, cancellationToken);
    }

    public async Task AddCategoryAsync(Category category, CancellationToken cancellationToken)
    {
        await dbContext.Categories.AddAsync(category, cancellationToken);
    }

    public void UpdateCategory(Category category)
    {
        dbContext.Categories.Update(category);
    }

    public async Task DeleteCategoryAsync(Category category, Category fallbackCategory, CancellationToken cancellationToken)
    {
        var videosToReassign = await dbContext.Videos
            .Where(video => video.CategoryId == category.Id)
            .ToListAsync(cancellationToken);

        foreach (var video in videosToReassign)
        {
            video.ChangeCategory(fallbackCategory.Id);
        }

        dbContext.Categories.Remove(category);
    }

    public Task<Tag?> GetTagByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.Tags
            .Include(tag => tag.VideoTags)
            .FirstOrDefaultAsync(tag => tag.Id == id, cancellationToken);
    }

    public Task<Tag?> GetTagByNameAsync(string name, CancellationToken cancellationToken)
    {
        return dbContext.Tags
            .FirstOrDefaultAsync(tag => tag.Name.ToLower() == name.ToLower(), cancellationToken);
    }

    public Task<Tag?> GetDefaultTagAsync(CancellationToken cancellationToken)
    {
        return dbContext.Tags
            .FirstOrDefaultAsync(tag => tag.Slug == Tag.DefaultSlug, cancellationToken);
    }

    public async Task AddTagAsync(Tag tag, CancellationToken cancellationToken)
    {
        await dbContext.Tags.AddAsync(tag, cancellationToken);
    }

    public void UpdateTag(Tag tag)
    {
        dbContext.Tags.Update(tag);
    }

    public async Task DeleteTagAsync(Tag tag, Tag fallbackTag, CancellationToken cancellationToken)
    {
        var videoIdsWithTag = await dbContext.VideoTags
            .Where(vt => vt.TagId == tag.Id)
            .Select(vt => vt.VideoId)
            .ToListAsync(cancellationToken);

        var links = dbContext.VideoTags.Where(vt => vt.TagId == tag.Id);
        dbContext.VideoTags.RemoveRange(links);
        dbContext.Tags.Remove(tag);

        foreach (var videoId in videoIdsWithTag)
        {
            var hasOtherTags = await dbContext.VideoTags
                .AnyAsync(vt => vt.VideoId == videoId && vt.TagId != tag.Id, cancellationToken);
            if (!hasOtherTags)
            {
                await dbContext.VideoTags.AddAsync(new VideoTag(videoId, fallbackTag.Id), cancellationToken);
            }
        }
    }

    public Task<Video?> GetVideoByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.Videos
            .Include(video => video.Creator)
            .Include(video => video.Category)
            .Include(video => video.VideoTags)
                .ThenInclude(link => link.Tag)
            .FirstOrDefaultAsync(video => video.Id == id, cancellationToken);
    }

    public void UpdateVideo(Video video)
    {
        dbContext.Videos.Update(video);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<Video> VideosQuery()
    {
        return dbContext.Videos
            .AsNoTracking()
            .AsSplitQuery()
            .Include(video => video.Creator)
            .Include(video => video.Category)
            .Include(video => video.VideoTags)
                .ThenInclude(link => link.Tag);
    }
}
