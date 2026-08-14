using SharedKernal.Messaging;
using SharedKernal.Results;

namespace Profiles.Application.ActorProfiles.Commands.DeleteActorProfile;

internal sealed record DeleteActorProfileCommand(Guid ProfileId) : ICommand<Result<bool>>;