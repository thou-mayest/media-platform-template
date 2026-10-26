using Catalog.Application.Abstractions;
using Catalog.Domain;
using SharedKernal.Messaging;

namespace Catalog.Application.Catalog;

public sealed record UpdateCategoryCommand(Guid Id, string Name, string? Slug = null) : ICommand<CategoryDto?>;

internal sealed class UpdateCategoryCommandHandler(ICatalogRepository repository)
    : ICommandHandler<UpdateCategoryCommand, CategoryDto?>
{
    public async Task<CategoryDto?> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await repository.GetCategoryByIdAsync(request.Id, cancellationToken);
        if (category is null)
        {
            return null;
        }

        var slug = string.IsNullOrWhiteSpace(request.Slug)
            ? SlugHelper.Slugify(request.Name)
            : SlugHelper.Slugify(request.Slug);

        if (category.Slug.Equals(Category.DefaultSlug, StringComparison.OrdinalIgnoreCase) &&
            !slug.Equals(Category.DefaultSlug, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("The default uncategorized category slug cannot be changed.");
        }

        var existingWithSlug = await repository.GetCategoryBySlugAsync(slug, cancellationToken);
        if (existingWithSlug is not null && existingWithSlug.Id != category.Id)
        {
            throw new InvalidOperationException($"A category with slug '{slug}' already exists.");
        }

        var existingWithName = await repository.GetCategoryByNameAsync(request.Name, cancellationToken);
        if (existingWithName is not null && existingWithName.Id != category.Id)
        {
            throw new InvalidOperationException($"A category with name '{request.Name}' already exists.");
        }

        category.Update(request.Name, slug);
        repository.UpdateCategory(category);
        await repository.SaveChangesAsync(cancellationToken);

        return category.ToCategoryDto();
    }
}

