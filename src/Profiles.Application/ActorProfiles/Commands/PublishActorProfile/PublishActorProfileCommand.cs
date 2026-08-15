using SharedKernal.Messaging;
using SharedKernal.Results;

namespace Profiles.Application.ActorProfiles.Commands.PublishActorProfile;

internal sealed record PublishActorProfileCommand(Guid ProfileId) : ICommand<Result<ActorProfileDto>>;