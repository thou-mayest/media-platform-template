namespace Catalog.Application.Catalog;

public sealed record CreatorCardDto(
    Guid Id,
    string Name,
    string Location,
    string Disciplines,
    string Bio,
    string Accent,
    string Initials,
    string Since,
    string Slug,
    int WorkCount,
    IReadOnlyList<string> SampleBackgrounds);

public sealed record CreatorMiniDto(Guid Id, string Name, string Slug);

public sealed record VideoCardDto(
    Guid Id,
    Guid CreatorId,
    string CreatorName,
    string CreatorSlug,
    string Title,
    int Year,
    string Category,
    string CategorySlug,
    string Medium,
    IReadOnlyList<string> Tags,
    double AspectRatio,
    string Background,
    string Size,
    string Slug);

public sealed record CategorySummaryDto(
    string Name,
    int Count,
    string Background,
    string Slug);

public sealed record SpotlightFeedDto(
    CreatorCardDto Creator,
    IReadOnlyList<VideoCardDto> Works);

public sealed record HomeFeedDto(
    IReadOnlyList<VideoCardDto> HeroWorks,
    SpotlightFeedDto Spotlight,
    IReadOnlyList<VideoCardDto> RecentWorks,
    IReadOnlyList<CategorySummaryDto> Categories);

public sealed record CatalogSnapshotDto(
    IReadOnlyList<CreatorCardDto> Creators,
    IReadOnlyList<VideoCardDto> Works,
    IReadOnlyList<CategorySummaryDto> Categories,
    IReadOnlyList<string> Tags);

public sealed record CreatorProfileDto(
    CreatorCardDto Creator,
    IReadOnlyList<VideoCardDto> Works,
    IReadOnlyList<string> Tags);

public sealed record CategoryDetailDto(
    CategorySummaryDto Category,
    IReadOnlyList<VideoCardDto> Works,
    IReadOnlyList<string> Tags);

public sealed record VideoDetailDto(
    VideoCardDto Video,
    CreatorCardDto Creator,
    IReadOnlyList<VideoCardDto> Related);

public sealed record TagDetailDto(
    string Name,
    string Slug,
    IReadOnlyList<CreatorMiniDto> Creators,
    IReadOnlyList<VideoCardDto> Works);

public sealed record SearchResultsDto(
    string Query,
    IReadOnlyList<CreatorCardDto> Creators,
    IReadOnlyList<VideoCardDto> Works);

public sealed record CategoryDto(
    Guid Id,
    string Name,
    string Slug,
    int WorkCount);

public sealed record TagDto(
    Guid Id,
    string Name,
    string Slug,
    int WorkCount);
