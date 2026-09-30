using SharedKernal.Messaging.Queries;
using SharedKernal.Results;

namespace Users.Application.Users.Queries.GetAllUsers;

internal sealed record GetAllUsersQuery : IQuery<Result<IReadOnlyList<UserDto>>>;
