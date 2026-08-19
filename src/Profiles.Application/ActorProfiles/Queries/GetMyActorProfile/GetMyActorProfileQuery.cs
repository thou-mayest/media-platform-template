using SharedKernal.Messaging;
using SharedKernal.Results;

namespace Profiles.Application.ActorProfiles.Queries.GetMyActorProfile;

internal sealed record GetMyActorProfileQuery(Guid UserId) : IQuery<Result<ActorProfileDto>>;