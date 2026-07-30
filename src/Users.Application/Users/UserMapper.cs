using Users.Domain;

namespace Users.Application.Users;

internal static class UserMapper
{
    internal static UserDto ToDto(this User user) =>
        new(user.Id, user.Name, user.Email, user.Role, user.CreatedDate, user.UpdateDate);
}
