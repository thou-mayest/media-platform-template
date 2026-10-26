using Catalog.Application.Abstractions;
using SharedKernal.Messaging;

namespace Catalog.Application.Catalog;

public sealed record GetAllCategoriesQuery : IQuery<IReadOnlyList<CategoryDto>>;

internal sealed class GetAllCategoriesQueryHandler(ICatalogRepository repository)
    : IQueryHandler<GetAllCategoriesQuery, IReadOnlyList<CategoryDto>>
{
    public async Task<IReadOnlyList<CategoryDto>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await repository.GetCategoriesAsync(cancellationToken);
        return categories.Select(c => c.ToCategoryDto()).ToList();
    }
}

