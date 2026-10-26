using Catalog.Application.Abstractions;
using SharedKernal.Messaging;

namespace Catalog.Application.Catalog;

public sealed record GetAllTagsQuery : IQuery<IReadOnlyList<TagDto>>;

internal sealed class GetAllTagsQueryHandler(ICatalogRepository repository)
    : IQueryHandler<GetAllTagsQuery, IReadOnlyList<TagDto>>
{
    public async Task<IReadOnlyList<TagDto>> Handle(GetAllTagsQuery request, CancellationToken cancellationToken)
    {
        var tags = await repository.GetTagsAsync(cancellationToken);
        return tags.Select(t => t.ToTagDto()).ToList();
    }
}

