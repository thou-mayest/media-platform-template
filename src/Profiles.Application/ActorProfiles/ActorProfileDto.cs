using Profiles.Domain.ValueObjects;

namespace Profiles.Application.ActorProfiles;

internal sealed record ActorProfileDto(
    Guid Id,
    string Slug,
    string DisplayName,
    string Profession,
    string Bio,
    string? AvatarStorageKey,
    IReadOnlyList<SocialLinkDto> SocialLinks,
    int AlbumCount,
    int MediaCount,
    int FollowerCount,
    bool IsPublished,
    DateTime CreatedDate,
    DateTime? UpdateDate);

internal sealed record SocialLinkDto(SocialPlatform Platform, string Url);