using SharedKernal.Messaging;

namespace Users.Application.Users.Queries.GetAllUsers;

internal sealed record GetAllUsersQuery(int Page, int PageSize) : IQuery<IReadOnlyList<UserDto>>;
