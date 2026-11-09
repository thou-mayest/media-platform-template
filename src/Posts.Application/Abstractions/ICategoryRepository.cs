using Posts.Domain;

namespace Posts.Application.Abstractions;

internal interface ICategoryRepository
{
    Task<List<Category>> GetAllAsync(CancellationToken cancellationToken);
    Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> ExistsByNormalizedNameAsync(string normalizedName, CancellationToken cancellationToken);
    Task<bool> HasPostsAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(Category category, CancellationToken cancellationToken);
    void Remove(Category category);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
