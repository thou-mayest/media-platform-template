using Profiles.Application.Abstractions;
using SharedKernal.Messaging;
using SharedKernal.Pagination;
using SharedKernal.Results;

namespace Profiles.Application.ActorProfiles.Queries.GetPublishedActorProfiles;

internal sealed class GetPublishedActorProfilesQueryHandler(IActorProfileRepository repository)
    : IQueryHandler<GetPublishedActorProfilesQuery, Result<PagedResult<ActorProfileDto>>>
{
    public async Task<Result<PagedResult<ActorProfileDto>>> Handle(
        GetPublishedActorProfilesQuery request, CancellationToken cancellationToken)
    {
        var page = await repository.GetPublishedAsync(request.Page, cancellationToken);

        if (page.Items.Count == 0 && page.Page > 1)
            return Error.NotFound("ActorProfile.PageOutOfRange", $"Page {page.Page} does not exist.");

        return page.Map(p => p.ToDto());
    }
}