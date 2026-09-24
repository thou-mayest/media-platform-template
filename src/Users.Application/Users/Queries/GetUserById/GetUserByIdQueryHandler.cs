using Microsoft.Extensions.Caching.Hybrid;
using SharedKernal.Messaging;
using SharedKernal.Results;
using Users.Application.Abstractions;
using Users.Domain;

namespace Users.Application.Users.Queries.GetUserById;

internal sealed class GetUserByIdQueryHandler(IUserRepository userRepository, HybridCache cache)
    : IQueryHandler<GetUserByIdQuery, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await cache.GetOrCreateAsync(
            UserCacheKeys.UserById(request.Id),
            async ct =>
            {
                var user = await userRepository.GetByIdAsync(request.Id, ct);
                return user?.ToDto();
            },
            tags: [UserCacheKeys.Tag],
            cancellationToken: cancellationToken);

        if (user is null)
        {
            return Error.NotFound(ErrorCodes.NotFound, "user not found");
        }

        return Result.Success(user);
    }
}