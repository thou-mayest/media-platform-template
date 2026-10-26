using Catalog.Application.Abstractions;
using Catalog.Domain;
using SharedKernal.Messaging;

namespace Catalog.Application.Catalog;

public sealed record DeleteCategoryCommand(Guid Id) : ICommand<bool>;

internal sealed class DeleteCategoryCommandHandler(ICatalogRepository repository)
    : ICommandHandler<DeleteCategoryCommand, bool>
{
    public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await repository.GetCategoryByIdAsync(request.Id, cancellationToken);
        if (category is null)
        {
            return false;
        }

        if (category.Slug.Equals(Category.DefaultSlug, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("The default uncategorized category cannot be deleted.");
        }

        var defaultCategory = await repository.GetDefaultCategoryAsync(cancellationToken);
        if (defaultCategory is null)
        {
            defaultCategory = new Category(Category.DefaultName, Category.DefaultSlug);
            await repository.AddCategoryAsync(defaultCategory, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);
        }

        await repository.DeleteCategoryAsync(category, defaultCategory, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return true;
    }
}

