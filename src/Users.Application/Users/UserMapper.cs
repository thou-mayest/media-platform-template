using Riok.Mapperly.Abstractions;
using Users.Domain;

namespace Users.Application.Users;

[Mapper]
internal static partial class UserMapper
{

    internal static partial UserDto ToDto(this User user);
}