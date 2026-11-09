using Posts.Application.Abstractions;
using SharedKernal.Messaging.Queries;
using SharedKernal.Results;

namespace Posts.Application.Categories.Queries.GetCategoryById;

internal sealed class GetCategoryByIdQueryHandler(ICategoryRepository repository)
    : IQueryHandler<GetCategoryByIdQuery, Result<CategoryDto>>
{
    public async Task<Result<CategoryDto>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (category is null)
            return Error.NotFound(ErrorCodes.NotFound, $"Category with Id {request.Id} not found.");

        return new CategoryDto(category.Id, category.Name);
    }
}
