using Catalog.Application.Abstractions;
using SharedKernal.Messaging;

namespace Catalog.Application.Catalog;

public sealed record GetCreatorProfileQuery(string Slug) : IQuery<CreatorProfileDto?>;

internal sealed class GetCreatorProfileQueryHandler(ICatalogRepository catalogRepository)
    : IQueryHandler<GetCreatorProfileQuery, CreatorProfileDto?>
{
    public async Task<CreatorProfileDto?> Handle(GetCreatorProfileQuery request, CancellationToken cancellationToken)
    {
        var creator = await catalogRepository.GetCreatorBySlugAsync(request.Slug, cancellationToken);
        if (creator is null)
        {
            return null;
        }

        var works = creator.Videos
            .OrderBy(video => video.DisplayOrder)
            .Select(video => video.ToVideoCardDto(creator, video.Category))
            .ToList();

        var tags = creator.Videos
            .SelectMany(video => video.VideoTags)
            .Select(link => link.Tag.Name)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(name => name)
            .ToList();

        return new CreatorProfileDto(
            creator.ToCreatorCardDto(),
            works,
            tags);
    }
}
