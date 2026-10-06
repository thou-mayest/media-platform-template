using Profiles.Contracts;

namespace Profiles.Presentation.Profiles;


public sealed record ProfileResponse(
    Guid Id,
    string Slug,
    string DisplayName,
    string Profession,
    string Bio,
    string? AvatarStorageKey,
    IReadOnlyList<SocialLinkResponse> SocialLinks,
    int AlbumCount,
    int MediaCount,
    int FollowerCount,
    bool IsPublished,
    bool IsIndexable,
    DateTime CreatedDate,
    DateTime? UpdateDate);

public sealed record SocialLinkResponse(SocialPlatform Platform, string Url);