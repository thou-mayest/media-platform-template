using Posts.Application.Abstractions;
using SharedKernal.Messaging.Queries;
using SharedKernal.Results;

namespace Posts.Application.Posts.Queries.GetPostById;

internal sealed class GetPostByIdQueryHandler(IPostRepository repository)
    : IQueryHandler<GetPostByIdQuery, Result<PostDto>>
{
    public async Task<Result<PostDto>> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
    {
        var post = await repository.GetPublishedByIdAsync(request.Id, cancellationToken);
        return post is null
            ? Error.NotFound("Post.NotFound", $"Published post with id {request.Id} was not found.")
            : post;
    }
}
