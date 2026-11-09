using Posts.Application.Abstractions;
using Posts.Domain;
using SharedKernal.Messaging.Commands;
using SharedKernal.Results;

namespace Posts.Application.Categories.Commands.DeleteCategory;

internal sealed class DeleteCategoryCommandHandler(ICategoryRepository repository)
    : ICommandHandler<DeleteCategoryCommand, Result>
{
    public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (category is null)
            return Error.NotFound(ErrorCodes.NotFound, $"Category with Id {request.Id} not found.");

        if (category.Id == Category.DefaultId)
            return Error.Conflict("Category.CannotDeleteDefault", "The default category cannot be deleted.");

        if (await repository.HasPostsAsync(category.Id, cancellationToken))
            return Error.Conflict("Category.HasPosts", "Cannot delete a category that has posts assigned to it.");

        repository.Remove(category);
        await repository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
