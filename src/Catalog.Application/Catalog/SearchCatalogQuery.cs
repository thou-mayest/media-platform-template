using Catalog.Application.Abstractions;
using SharedKernal.Messaging;

namespace Catalog.Application.Catalog;

public sealed record SearchCatalogQuery(string Query) : IQuery<SearchResultsDto>;

internal sealed class SearchCatalogQueryHandler(ICatalogRepository catalogRepository)
    : IQueryHandler<SearchCatalogQuery, SearchResultsDto>
{
    public async Task<SearchResultsDto> Handle(SearchCatalogQuery request, CancellationToken cancellationToken)
    {
        var query = request.Query.Trim();
        if (string.IsNullOrWhiteSpace(query))
        {
            return new SearchResultsDto(string.Empty, [], []);
        }

        var comparison = StringComparison.OrdinalIgnoreCase;
        var creators = await catalogRepository.GetCreatorsAsync(cancellationToken);
        var videos = await catalogRepository.GetVideosAsync(cancellationToken);

        var matchedCreators = creators
            .Where(creator =>
                creator.Name.Contains(query, comparison) ||
                creator.Location.Contains(query, comparison) ||
                creator.Disciplines.Contains(query, comparison) ||
                creator.Bio.Contains(query, comparison))
            .OrderBy(creator => creator.Name)
            .Select(creator => creator.ToCreatorCardDto())
            .ToList();

        var matchedVideos = videos
            .Where(video =>
                video.Title.Contains(query, comparison) ||
                video.Creator.Name.Contains(query, comparison) ||
                video.Category.Name.Contains(query, comparison) ||
                video.VideoTags.Any(link => link.Tag.Name.Contains(query, comparison)))
            .OrderBy(video => video.DisplayOrder)
            .Select(video => video.ToVideoCardDto())
            .ToList();

        return new SearchResultsDto(query, matchedCreators, matchedVideos);
    }
}
