using Profiles.Application.Abstractions;
using SharedKernal.Messaging;
using SharedKernal.Results;

namespace Profiles.Application.ActorProfiles.Commands.UnpublishActorProfile;

internal sealed class UnpublishActorProfileCommandHandler(IActorProfileRepository repository)
    : ICommandHandler<UnpublishActorProfileCommand, Result<ActorProfileDto>>
{
    public async Task<Result<ActorProfileDto>> Handle(
        UnpublishActorProfileCommand request, CancellationToken cancellationToken)
    {
        var profile = await repository.GetByUserIdAsync(request.UserId, cancellationToken);

        if (profile is null)
            return Error.NotFound("ActorProfile.NotFound", "Profile not found.");

        var result = profile.Unpublish();
        if (result.IsFailure)
            return Result.Failure<ActorProfileDto>(result.Errors);

        repository.Update(profile);
        await repository.SaveChangesAsync(cancellationToken);
        return profile.ToDto();
    }
}