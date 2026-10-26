using Catalog.Domain;

namespace Catalog.Application.Catalog;

internal static class CatalogMappings
{
    public static CreatorCardDto ToCreatorCardDto(this Creator creator)
    {
        var orderedVideos = creator.Videos
            .OrderBy(video => video.DisplayOrder)
            .ToList();

        return new CreatorCardDto(
            creator.Id,
            creator.Name,
            creator.Location,
            creator.Disciplines,
            creator.Bio,
            creator.Accent,
            creator.Initials,
            creator.Since,
            creator.Slug,
            orderedVideos.Count,
            orderedVideos
                .Take(3)
                .Select(video => video.Background)
                .ToList());
    }

    public static CreatorMiniDto ToCreatorMiniDto(this Creator creator) =>
        new(creator.Id, creator.Name, creator.Slug);

    public static VideoCardDto ToVideoCardDto(
        this Video video,
        Creator? creatorOverride = null,
        Category? categoryOverride = null)
    {
        var creator = creatorOverride ?? video.Creator;
        var category = categoryOverride ?? video.Category;

        return new VideoCardDto(
            video.Id,
            video.CreatorId,
            creator.Name,
            creator.Slug,
            video.Title,
            video.Year,
            category.Name,
            category.Slug,
            video.Medium,
            video.VideoTags
                .Select(link => link.Tag.Name)
                .OrderBy(name => name)
                .ToList(),
            video.AspectRatio,
            video.Background,
            video.Size,
            video.Slug);
    }

    public static CategorySummaryDto ToCategorySummaryDto(this Category category)
    {
        var orderedVideos = category.Videos
            .OrderBy(video => video.DisplayOrder)
            .ToList();

        return new CategorySummaryDto(
            category.Name,
            orderedVideos.Count,
            orderedVideos.FirstOrDefault()?.Background ?? "var(--surface)",
            category.Slug);
    }

    public static CategoryDto ToCategoryDto(this Category category) =>
        new(category.Id, category.Name, category.Slug, category.Videos.Count);

    public static TagDto ToTagDto(this Tag tag) =>
        new(tag.Id, tag.Name, tag.Slug, tag.VideoTags.Count);
}
