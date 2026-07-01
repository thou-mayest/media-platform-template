using SharedKernal.Messaging;

namespace Users.Application.Users.Queries.GetAllUsers;

internal sealed record GetAllUsersQuery : IQuery<IReadOnlyList<UserDto>>;
