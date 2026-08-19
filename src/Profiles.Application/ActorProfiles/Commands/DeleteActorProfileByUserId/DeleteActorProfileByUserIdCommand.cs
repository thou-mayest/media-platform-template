using SharedKernal.Messaging;
using SharedKernal.Results;

namespace Profiles.Application.ActorProfiles.Commands.DeleteActorProfileByUserId;


internal sealed record DeleteActorProfileByUserIdCommand(Guid UserId) : ICommand<Result<bool>>;
