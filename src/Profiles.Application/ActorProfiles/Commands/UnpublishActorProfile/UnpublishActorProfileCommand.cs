using SharedKernal.Messaging;
using SharedKernal.Results;

namespace Profiles.Application.ActorProfiles.Commands.UnpublishActorProfile;

internal sealed record UnpublishActorProfileCommand(Guid ProfileId) : ICommand<Result<ActorProfileDto>>;