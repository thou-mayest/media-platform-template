using Catalog.Application.Abstractions;
using Catalog.Domain;
using SharedKernal.Messaging;

namespace Catalog.Application.Catalog;

public sealed record CreateCategoryCommand(string Name, string? Slug = null) : ICommand<CategoryDto>;

internal sealed class CreateCategoryCommandHandler(ICatalogRepository repository)
    : ICommandHandler<CreateCategoryCommand, CategoryDto>
{
    public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var slug = string.IsNullOrWhiteSpace(request.Slug)
            ? SlugHelper.Slugify(request.Name)
            : SlugHelper.Slugify(request.Slug);

        var existingWithSlug = await repository.GetCategoryBySlugAsync(slug, cancellationToken);
        if (existingWithSlug is not null)
        {
            throw new InvalidOperationException($"A category with slug '{slug}' already exists.");
        }

        var existingWithName = await repository.GetCategoryByNameAsync(request.Name, cancellationToken);
        if (existingWithName is not null)
        {
            throw new InvalidOperationException($"A category with name '{request.Name}' already exists.");
        }

        var category = new Category(request.Name, slug);
        await repository.AddCategoryAsync(category, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return category.ToCategoryDto();
    }
}

