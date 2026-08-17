using Profiles.Domain.ValueObjects;
using SharedKernal.Messaging;
using SharedKernal.Results;

namespace Profiles.Application.ActorProfiles.Commands.UpdateActorProfile;


internal sealed record UpdateActorProfileCommand(
    Guid UserId,
    string DisplayName,
    string? Profession,
    string? Bio,
    string? Slug,
    string? AvatarStorageKey,
    IReadOnlyList<SocialLinkInput> SocialLinks) : ICommand<Result<ActorProfileDto>>;

internal sealed record SocialLinkInput(SocialPlatform Platform, string? Url);