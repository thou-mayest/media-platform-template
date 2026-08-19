using Profiles.Application.Abstractions;
using SharedKernal.Messaging;
using SharedKernal.Results;

namespace Profiles.Application.ActorProfiles.Queries.GetActorProfileBySlug;

internal sealed class GetActorProfileBySlugQueryHandler(IActorProfileRepository repository)
    : IQueryHandler<GetActorProfileBySlugQuery, Result<ActorProfileDto>>
{
    public async Task<Result<ActorProfileDto>> Handle(
        GetActorProfileBySlugQuery request, CancellationToken cancellationToken)
    {
        
        var slug = request.Slug?.Trim().ToLowerInvariant();

        if (string.IsNullOrEmpty(slug))
            return Error.NotFound("ActorProfile.NotFound", "Profile not found.");

        var profile = await repository.GetPublishedBySlugAsync(slug, cancellationToken);

        if (profile is null)
            return Error.NotFound("ActorProfile.NotFound", "Profile not found.");

        return profile.ToDto();
    }
}