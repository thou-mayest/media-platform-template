using Catalog.Application.Abstractions;
using SharedKernal.Messaging;

namespace Catalog.Application.Catalog;

public sealed record GetCategoryDetailQuery(string Slug) : IQuery<CategoryDetailDto?>;

internal sealed class GetCategoryDetailQueryHandler(ICatalogRepository catalogRepository)
    : IQueryHandler<GetCategoryDetailQuery, CategoryDetailDto?>
{
    public async Task<CategoryDetailDto?> Handle(GetCategoryDetailQuery request, CancellationToken cancellationToken)
    {
        var category = await catalogRepository.GetCategoryBySlugAsync(request.Slug, cancellationToken);
        if (category is null)
        {
            return null;
        }

        var works = category.Videos
            .OrderBy(video => video.DisplayOrder)
            .Select(video => video.ToVideoCardDto(video.Creator, category))
            .ToList();

        var tags = category.Videos
            .SelectMany(video => video.VideoTags)
            .Select(link => link.Tag.Name)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(name => name)
            .ToList();

        return new CategoryDetailDto(
            category.ToCategorySummaryDto(),
            works,
            tags);
    }
}
