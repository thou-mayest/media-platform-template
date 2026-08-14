using Profiles.Application.Abstractions;
using SharedKernal.Messaging;
using SharedKernal.Results;

namespace Profiles.Application.ActorProfiles.Queries.GetMyActorProfile;


internal sealed class GetMyActorProfileQueryHandler(IActorProfileRepository repository)
    : IQueryHandler<GetMyActorProfileQuery, Result<ActorProfileDto>>
{
    public async Task<Result<ActorProfileDto>> Handle(
        GetMyActorProfileQuery request, CancellationToken cancellationToken)
    {
        var profile = await repository.GetByUserIdAsync(request.UserId, cancellationToken);

        if (profile is null)
            return Error.NotFound("ActorProfile.NotFound", "You do not have a profile yet.");

        return profile.ToDto();
    }
}