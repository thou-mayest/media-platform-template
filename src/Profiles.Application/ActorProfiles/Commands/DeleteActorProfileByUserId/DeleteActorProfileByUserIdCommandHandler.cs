using Profiles.Application.Abstractions;
using SharedKernal.Messaging;
using SharedKernal.Results;

namespace Profiles.Application.ActorProfiles.Commands.DeleteActorProfileByUserId;

internal sealed class DeleteActorProfileByUserIdCommandHandler(IActorProfileRepository repository)
    : ICommandHandler<DeleteActorProfileByUserIdCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        DeleteActorProfileByUserIdCommand request, CancellationToken cancellationToken)
    {
        var profile = await repository.GetByUserIdAsync(request.UserId, cancellationToken);

       
        if (profile is null)
            return true;

        profile.Delete();
        repository.Remove(profile);
        await repository.SaveChangesAsync(cancellationToken);
        return true;
    }
}
