using Posts.Application.Abstractions;
using Posts.Domain;
using SharedKernal.Messaging.Commands;
using SharedKernal.Results;

namespace Posts.Application.Categories.Commands.CreateCategory;

internal sealed class CreateCategoryCommandHandler(ICategoryRepository repository)
    : ICommandHandler<CreateCategoryCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var result = Category.Create(request.Name);
        if (result.IsFailure)
            return result.Error;

        var category = result.Value;

        if (await repository.ExistsByNormalizedNameAsync(category.NormalizedName, cancellationToken))
            return Error.Conflict("Category.NameExists", "A category with that name already exists.");

        await repository.AddAsync(category, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return category.Id;
    }
}
