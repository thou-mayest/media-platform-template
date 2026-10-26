using Catalog.Application.Abstractions;
using Catalog.Domain;
using SharedKernal.Messaging;

namespace Catalog.Application.Catalog;

public sealed record GetHomeFeedQuery() : IQuery<HomeFeedDto>;

internal sealed class GetHomeFeedQueryHandler(ICatalogRepository catalogRepository)
    : IQueryHandler<GetHomeFeedQuery, HomeFeedDto>
{
    private static readonly int[] HeroDisplayOrders = [0, 24, 12];

    public async Task<HomeFeedDto> Handle(GetHomeFeedQuery request, CancellationToken cancellationToken)
    {
        var creators = await catalogRepository.GetCreatorsAsync(cancellationToken);
        var videos = await catalogRepository.GetVideosAsync(cancellationToken);
        var categories = await catalogRepository.GetCategoriesAsync(cancellationToken);

        var orderedVideos = videos
            .OrderBy(video => video.DisplayOrder)
            .ToList();

        var heroVideos = HeroDisplayOrders
            .Select(order => orderedVideos.FirstOrDefault(video => video.DisplayOrder == order))
            .Where(video => video is not null)
            .Select(video => video!.ToVideoCardDto())
            .ToList();

        var spotlightCreator = creators
            .FirstOrDefault(creator => string.Equals(creator.Slug, "mira-okafor", StringComparison.Ordinal))
            ?? creators
                .OrderByDescending(creator => creator.Videos.Count)
                .ThenBy(creator => creator.Name)
                .First();

        var spotlightWorks = spotlightCreator.Videos
            .OrderBy(video => video.DisplayOrder)
            .Take(3)
            .Select(video => video.ToVideoCardDto(spotlightCreator, video.Category))
            .ToList();

        var recentWorks = orderedVideos
            .OrderByDescending(video => video.Year)
            .ThenBy(video => video.DisplayOrder)
            .Take(12)
            .Select(video => video.ToVideoCardDto())
            .ToList();

        var orderedCategories = categories
            .OrderBy(category => category.Name)
            .Select(category => category.ToCategorySummaryDto())
            .ToList();

        return new HomeFeedDto(
            heroVideos,
            new SpotlightFeedDto(spotlightCreator.ToCreatorCardDto(), spotlightWorks),
            recentWorks,
            orderedCategories);
    }
}
