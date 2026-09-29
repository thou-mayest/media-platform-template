using Microsoft.Extensions.Caching.Hybrid;
using SharedKernal.Messaging;
using SharedKernal.Results;
using Users.Application.Abstractions;
using Users.Domain;

namespace Users.Application.Users.Queries.GetAllUsers;

internal sealed class GetAllUsersQueryHandler(IUserRepository userRepository, HybridCache cache)
    : IQueryHandler<GetAllUsersQuery, Result<IReadOnlyList<UserDto>>>
{
    public async Task<Result<IReadOnlyList<UserDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<UserDto> users = await cache.GetOrCreateAsync<IReadOnlyList<UserDto>>(
            UserCacheKeys.AllUsers,
            async ct =>
            {
                var users = await userRepository.GetAllAsync(ct);

                return users
                    .Select(u => u.ToDto())
                    .ToList();
            },
            tags: [UserCacheKeys.Tag],
            cancellationToken: cancellationToken);

        if (users is null || users.Count == 0)
        {
            return Error.NotFound(ErrorCodes.NotFound, "user list is empty");
        }

        return Result.Success(users);
    }       
}