using Profiles.Contracts;

namespace Profiles.Presentation.Profiles;


public sealed record UpdateProfileRequest(
    string DisplayName,
    string? Profession,
    string? Bio,
    string? Slug,
    string? AvatarStorageKey,
    IReadOnlyList<SocialLinkRequest>? SocialLinks);

public sealed record SocialLinkRequest(SocialPlatform Platform, string? Url);