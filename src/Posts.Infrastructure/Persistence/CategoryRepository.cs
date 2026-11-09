using Microsoft.EntityFrameworkCore;
using Posts.Application.Abstractions;
using Posts.Domain;

namespace Posts.Infrastructure.Persistence;

internal sealed class CategoryRepository(PostsDbContext context) : ICategoryRepository
{
    public Task<List<Category>> GetAllAsync(CancellationToken cancellationToken) =>
        context.Categories
            .AsNoTracking()
            .OrderBy(category => category.Name)
            .ToListAsync(cancellationToken);

    public Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        context.Categories.FirstOrDefaultAsync(category => category.Id == id, cancellationToken);

    public Task<bool> ExistsByNormalizedNameAsync(string normalizedName, CancellationToken cancellationToken) =>
        context.Categories.AnyAsync(category => category.NormalizedName == normalizedName, cancellationToken);

    public Task<bool> HasPostsAsync(Guid id, CancellationToken cancellationToken) =>
        context.Posts.AnyAsync(post => post.CategoryId == id, cancellationToken);

    public async Task AddAsync(Category category, CancellationToken cancellationToken)
    {
        await context.Categories.AddAsync(category, cancellationToken);
    }

    public void Remove(Category category)
    {
        context.Categories.Remove(category);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        context.SaveChangesAsync(cancellationToken);
}
