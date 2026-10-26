using Catalog.Domain;

namespace Catalog.Application.Abstractions;

public interface ICatalogRepository
{
    Task<IReadOnlyList<Creator>> GetCreatorsAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<Category>> GetCategoriesAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<Tag>> GetTagsAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<Video>> GetVideosAsync(CancellationToken cancellationToken);

    Task<Creator?> GetCreatorBySlugAsync(string slug, CancellationToken cancellationToken);

    Task<Category?> GetCategoryBySlugAsync(string slug, CancellationToken cancellationToken);

    Task<Tag?> GetTagBySlugAsync(string slug, CancellationToken cancellationToken);

    Task<Video?> GetVideoBySlugAsync(string slug, CancellationToken cancellationToken);

    Task<Category?> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<Category?> GetCategoryByNameAsync(string name, CancellationToken cancellationToken);

    Task<Category?> GetDefaultCategoryAsync(CancellationToken cancellationToken);

    Task AddCategoryAsync(Category category, CancellationToken cancellationToken);

    void UpdateCategory(Category category);

    Task DeleteCategoryAsync(Category category, Category fallbackCategory, CancellationToken cancellationToken);

    Task<Tag?> GetTagByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<Tag?> GetTagByNameAsync(string name, CancellationToken cancellationToken);

    Task<Tag?> GetDefaultTagAsync(CancellationToken cancellationToken);

    Task AddTagAsync(Tag tag, CancellationToken cancellationToken);

    void UpdateTag(Tag tag);

    Task DeleteTagAsync(Tag tag, Tag fallbackTag, CancellationToken cancellationToken);

    Task<Video?> GetVideoByIdAsync(Guid id, CancellationToken cancellationToken);

    void UpdateVideo(Video video);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
