using SharedKernal.Messaging.Queries;
using SharedKernal.Results;

namespace Profiles.Application.ActorProfiles.Queries.GetMyActorProfile;

internal sealed record GetMyActorProfileQuery(Guid UserId) : IQuery<Result<ActorProfileDto>>;