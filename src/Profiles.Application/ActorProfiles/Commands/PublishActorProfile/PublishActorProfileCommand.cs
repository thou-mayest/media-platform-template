using SharedKernal.Messaging;
using SharedKernal.Results;

namespace Profiles.Application.ActorProfiles.Commands.PublishActorProfile;

internal sealed record PublishActorProfileCommand(Guid UserId) : ICommand<Result<ActorProfileDto>>;