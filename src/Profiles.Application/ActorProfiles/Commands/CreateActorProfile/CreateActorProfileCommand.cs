using SharedKernal.Messaging;
using SharedKernal.Results;

namespace Profiles.Application.ActorProfiles.Commands.CreateActorProfile;

internal sealed record CreateActorProfileCommand(
    Guid UserId,
    string DisplayName) : ICommand<Result<Guid>>;