using SharedKernal.Messaging;
using SharedKernal.Pagination;
using SharedKernal.Results;

namespace Profiles.Application.ActorProfiles.Queries.GetIndexableActorProfiles;

internal sealed record GetIndexableActorProfilesQuery(PageRequest Page)
    : IQuery<Result<PagedResult<ActorProfileDto>>>;
