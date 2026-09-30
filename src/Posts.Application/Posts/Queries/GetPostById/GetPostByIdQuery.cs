using SharedKernal.Messaging.Queries;
using SharedKernal.Results;

namespace Posts.Application.Posts.Queries.GetPostById;

public sealed record GetPostByIdQuery(Guid Id) : IQuery<Result<PostDto>>;
