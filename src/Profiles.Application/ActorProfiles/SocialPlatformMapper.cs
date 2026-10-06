using Riok.Mapperly.Abstractions;

namespace Profiles.Application.ActorProfiles;


[Mapper]
internal static partial class SocialPlatformMapper
{
    internal static partial Profiles.Domain.ValueObjects.SocialPlatform ToDomain(
        this Profiles.Contracts.SocialPlatform platform);
}