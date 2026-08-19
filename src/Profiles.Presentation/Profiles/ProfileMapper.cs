using Profiles.Application.ActorProfiles;
using Profiles.Application.ActorProfiles.Commands.UpdateActorProfile;
using Profiles.Presentation.Common;
using Riok.Mapperly.Abstractions;
using SharedKernal.Pagination;

namespace Profiles.Presentation.Profiles;

[Mapper]
internal static partial class ProfileMapper
{
    internal static partial ProfileResponse ToResponse(this ActorProfileDto dto);

    private static partial SocialLinkResponse ToResponse(SocialLinkDto dto);

    internal static UpdateActorProfileCommand ToCommand(this UpdateProfileRequest request, Guid userId)
        => new(
            userId,
            request.DisplayName,
            request.Profession,
            request.Bio,
            request.Slug,
            request.AvatarStorageKey,
            request.SocialLinks?.Select(l => new SocialLinkInput(l.Platform, l.Url)).ToList() ?? []);

    internal static PagedResponse<ProfileResponse> ToResponse(this PagedResult<ActorProfileDto> page)
        => new(
            page.Items.Select(dto => dto.ToResponse()).ToList(),
            page.Page,
            page.PageSize,
            page.TotalItems,
            page.TotalPages,
            page.HasPrev,
            page.HasNext);
}