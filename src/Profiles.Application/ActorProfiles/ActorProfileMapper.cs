using Profiles.Domain;
using Profiles.Domain.ValueObjects;
using Riok.Mapperly.Abstractions;

namespace Profiles.Application.ActorProfiles;

[Mapper]
internal static partial class ActorProfileMapper
{
    // Ignored explicitly rather than left to warn. UserId is the Users foreign
    // key and has no business reaching a client; DomainEvents and Version are
    // persistence and dispatch machinery. Naming them here means a genuinely
    // forgotten field still produces an RMG020 that is worth reading.
    [MapperIgnoreSource(nameof(ActorProfile.UserId))]
    [MapperIgnoreSource(nameof(ActorProfile.DomainEvents))]
    [MapperIgnoreSource(nameof(ActorProfile.Version))]
    internal static partial ActorProfileDto ToDto(this ActorProfile profile);

    private static partial SocialLinkDto ToDto(SocialLink link);

    private static string MapSlug(ProfileSlug slug) => slug.Value;
}