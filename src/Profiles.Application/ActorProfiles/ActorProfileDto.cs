using Profiles.Domain.ValueObjects;

namespace Profiles.Application.ActorProfiles;

/// <summary>
/// Backend data only. AvatarStorageKey is a storage key, never a URL — the
/// frontend's image service resolves keys into CDN URLs, so the API stays
/// independent of which CDN is in front of it.
///
/// IsPublished and IsIndexable are separate: the first decides whether the page
/// resolves at all, the second whether it gets index,follow and a sitemap entry.
/// </summary>
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
    bool IsIndexable,
    DateTime CreatedDate,
    DateTime? UpdateDate);

internal sealed record SocialLinkDto(SocialPlatform Platform, string Url);
