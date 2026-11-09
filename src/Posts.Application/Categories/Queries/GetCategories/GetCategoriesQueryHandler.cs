using Posts.Application.Abstractions;
using SharedKernal.Messaging.Queries;
using SharedKernal.Results;

namespace Posts.Application.Categories.Queries.GetCategories;

internal sealed class GetCategoriesQueryHandler(ICategoryRepository repository)
    : IQueryHandler<GetCategoriesQuery, Result<IReadOnlyList<CategoryDto>>>
{
    public async Task<Result<IReadOnlyList<CategoryDto>>> Handle(
        GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await repository.GetAllAsync(cancellationToken);

        IReadOnlyList<CategoryDto> dtos = categories
            .Select(category => new CategoryDto(category.Id, category.Name))
            .ToList();

        return Result.Success(dtos);
    }
}
