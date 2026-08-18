using Profiles.Application.Abstractions;
using SharedKernal.Messaging;
using SharedKernal.Results;

namespace Profiles.Application.ActorProfiles.Commands.PublishActorProfile;

internal sealed class PublishActorProfileCommandHandler(IActorProfileRepository repository)
    : ICommandHandler<PublishActorProfileCommand, Result<ActorProfileDto>>
{
    public async Task<Result<ActorProfileDto>> Handle(
        PublishActorProfileCommand request, CancellationToken cancellationToken)
    {
        var profile = await repository.GetByUserIdAsync(request.UserId, cancellationToken);

        if (profile is null)
            return Error.NotFound("ActorProfile.NotFound", "Profile not found.");

        var result = profile.Publish();
        if (result.IsFailure)
            return Result.Failure<ActorProfileDto>(result.Errors);

        await repository.SaveChangesAsync(cancellationToken);
        return profile.ToDto();
    }
}