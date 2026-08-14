using Profiles.Application.Abstractions;
using SharedKernal.Messaging;
using SharedKernal.Results;

namespace Profiles.Application.ActorProfiles.Commands.DeleteActorProfile;

internal sealed class DeleteActorProfileCommandHandler(IActorProfileRepository repository)
    : ICommandHandler<DeleteActorProfileCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        DeleteActorProfileCommand request, CancellationToken cancellationToken)
    {
        var profile = await repository.GetByIdAsync(request.ProfileId, cancellationToken);

        if (profile is null)
            return true;

        profile.Delete();
        repository.Remove(profile);
        await repository.SaveChangesAsync(cancellationToken);
        return true;
    }
}