using Profiles.Application.Abstractions;
using SharedKernal.Messaging;
using SharedKernal.Pagination;
using SharedKernal.Results;

namespace Profiles.Application.ActorProfiles.Queries.GetIndexableActorProfiles;

/// <summary>
/// Backs both the actors directory and the sitemap. Filters to indexable rather
/// than merely published: a published-but-thin profile stays reachable at its
/// own URL, it is just not advertised to crawlers.
/// </summary>
internal sealed class GetIndexableActorProfilesQueryHandler(IActorProfileRepository repository)
    : IQueryHandler<GetIndexableActorProfilesQuery, Result<PagedResult<ActorProfileDto>>>
{
    public async Task<Result<PagedResult<ActorProfileDto>>> Handle(
        GetIndexableActorProfilesQuery request, CancellationToken cancellationToken)
    {
        var page = await repository.GetIndexableAsync(request.Page, cancellationToken);

        // An empty page 1 is a legitimate empty directory and must stay a 200.
        // An empty page 2+ means the caller walked past the end, and returning
        // 200 there gives crawlers an unbounded supply of indexable empty pages.
        if (page.Items.Count == 0 && page.Page > 1)
            return Error.NotFound("ActorProfile.PageOutOfRange", $"Page {page.Page} does not exist.");

        return page.Map(p => p.ToDto());
    }
}
