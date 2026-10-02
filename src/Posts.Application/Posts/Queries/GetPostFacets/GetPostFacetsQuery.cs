using SharedKernal.Messaging.Queries;

namespace Posts.Application.Posts.Queries.GetPostFacets;

public sealed record GetPostFacetsQuery : IQuery<PostFacetsDto>;
