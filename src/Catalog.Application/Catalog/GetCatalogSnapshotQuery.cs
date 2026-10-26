using Catalog.Application.Abstractions;
using SharedKernal.Messaging;

namespace Catalog.Application.Catalog;

public sealed record GetCatalogSnapshotQuery() : IQuery<CatalogSnapshotDto>;

internal sealed class GetCatalogSnapshotQueryHandler(ICatalogRepository catalogRepository)
    : IQueryHandler<GetCatalogSnapshotQuery, CatalogSnapshotDto>
{
    public async Task<CatalogSnapshotDto> Handle(GetCatalogSnapshotQuery request, CancellationToken cancellationToken)
    {
        var creators = await catalogRepository.GetCreatorsAsync(cancellationToken);
        var videos = await catalogRepository.GetVideosAsync(cancellationToken);
        var categories = await catalogRepository.GetCategoriesAsync(cancellationToken);
        var tags = await catalogRepository.GetTagsAsync(cancellationToken);

        return new CatalogSnapshotDto(
            creators
                .OrderBy(creator => creator.Name)
                .Select(creator => creator.ToCreatorCardDto())
                .ToList(),
            videos
                .OrderBy(video => video.DisplayOrder)
                .Select(video => video.ToVideoCardDto())
                .ToList(),
            categories
                .OrderBy(category => category.Name)
                .Select(category => category.ToCategorySummaryDto())
                .ToList(),
            tags
                .Select(tag => tag.Name)
                .OrderBy(name => name)
                .ToList());
    }
}
