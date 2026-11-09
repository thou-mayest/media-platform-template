using Posts.Application.Abstractions;
using SharedKernal.Messaging.Commands;
using SharedKernal.Results;

namespace Posts.Application.Categories.Commands.UpdateCategory;

internal sealed class UpdateCategoryCommandHandler(ICategoryRepository repository)
    : ICommandHandler<UpdateCategoryCommand, Result>
{
    public async Task<Result> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (category is null)
            return Error.NotFound(ErrorCodes.NotFound, $"Category with Id {request.Id} not found.");

        var normalizedName = request.Name is null ? null : Domain.Category.Normalize(request.Name);
        if (normalizedName is not null && normalizedName != category.NormalizedName &&
            await repository.ExistsByNormalizedNameAsync(normalizedName, cancellationToken))
            return Error.Conflict("Category.NameExists", "A category with that name already exists.");

        var updateResult = category.Update(request.Name);
        if (updateResult.IsFailure)
            return updateResult.Error;

        await repository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
