using Profiles.Application.Abstractions;
using Profiles.Domain;
using SharedKernal.Messaging;
using SharedKernal.Results;

namespace Profiles.Application.ActorProfiles.Commands.CreateActorProfile;

internal sealed class CreateActorProfileCommandHandler(IActorProfileRepository repository)
    : ICommandHandler<CreateActorProfileCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(
        CreateActorProfileCommand request, CancellationToken cancellationToken)
    {
        var existing = await repository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (existing is not null)
            return existing.Id;

        var slugResult = await ProfileSlugFactory.CreateUniqueAsync(
            repository, request.DisplayName, cancellationToken);

        if (slugResult.IsFailure)
            return slugResult.Error;

        var result = ActorProfile.Create(request.UserId, request.DisplayName, slugResult.Value);
        if (result.IsFailure)
            return result.Error;

        var profile = result.Value;
        await repository.AddAsync(profile, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return profile.Id;
    }
}