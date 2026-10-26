using Catalog.Application.Abstractions;
using SharedKernal.Messaging;

namespace Catalog.Application.Catalog;

public sealed record GetTagDetailQuery(string Slug) : IQuery<TagDetailDto?>;

internal sealed class GetTagDetailQueryHandler(ICatalogRepository catalogRepository)
    : IQueryHandler<GetTagDetailQuery, TagDetailDto?>
{
    public async Task<TagDetailDto?> Handle(GetTagDetailQuery request, CancellationToken cancellationToken)
    {
        var tag = await catalogRepository.GetTagBySlugAsync(request.Slug, cancellationToken);
        if (tag is null)
        {
            return null;
        }

        var videos = tag.VideoTags
            .Select(link => link.Video)
            .OrderBy(video => video.DisplayOrder)
            .ToList();

        var creators = videos
            .Select(video => video.Creator)
            .DistinctBy(creator => creator.Id)
            .OrderBy(creator => creator.Name)
            .Select(creator => creator.ToCreatorMiniDto())
            .ToList();

        return new TagDetailDto(
            tag.Name,
            tag.Slug,
            creators,
            videos.Select(video => video.ToVideoCardDto()).ToList());
    }
}
