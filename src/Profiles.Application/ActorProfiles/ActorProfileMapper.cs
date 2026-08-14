using Profiles.Domain;
using Profiles.Domain.ValueObjects;
using Riok.Mapperly.Abstractions;

namespace Profiles.Application.ActorProfiles;

[Mapper]
internal static partial class ActorProfileMapper
{
    internal static partial ActorProfileDto ToDto(this ActorProfile profile);

    private static partial SocialLinkDto ToDto(SocialLink link);

    private static string MapSlug(ProfileSlug slug) => slug.Value;
}