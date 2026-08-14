using SharedKernal.Messaging;
using SharedKernal.Pagination;
using SharedKernal.Results;

namespace Profiles.Application.ActorProfiles.Queries.GetPublishedActorProfiles;

internal sealed record GetPublishedActorProfilesQuery(PageRequest Page)
    : IQuery<Result<PagedResult<ActorProfileDto>>>;