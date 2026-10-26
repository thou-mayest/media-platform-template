using Catalog.Application.Abstractions;
using SharedKernal.Messaging;

namespace Catalog.Application.Catalog;

public sealed record GetVideoDetailQuery(string Slug) : IQuery<VideoDetailDto?>;

internal sealed class GetVideoDetailQueryHandler(ICatalogRepository catalogRepository)
    : IQueryHandler<GetVideoDetailQuery, VideoDetailDto?>
{
    public async Task<VideoDetailDto?> Handle(GetVideoDetailQuery request, CancellationToken cancellationToken)
    {
        var video = await catalogRepository.GetVideoBySlugAsync(request.Slug, cancellationToken);
        if (video is null)
        {
            return null;
        }

        var allVideos = await catalogRepository.GetVideosAsync(cancellationToken);
        var videoTagNames = video.VideoTags
            .Select(link => link.Tag.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var related = allVideos
            .Where(candidate =>
                candidate.Id != video.Id &&
                (candidate.CategoryId == video.CategoryId ||
                 candidate.VideoTags.Any(link => videoTagNames.Contains(link.Tag.Name))))
            .OrderBy(candidate => candidate.DisplayOrder)
            .Take(4)
            .Select(candidate => candidate.ToVideoCardDto())
            .ToList();

        return new VideoDetailDto(
            video.ToVideoCardDto(),
            video.Creator.ToCreatorCardDto(),
            related);
    }
}
