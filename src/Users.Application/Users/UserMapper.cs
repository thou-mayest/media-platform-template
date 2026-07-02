using Riok.Mapperly.Abstractions;
using Users.Domain;
using Users.Domain.ValueObjects;

namespace Users.Application.Users;

[Mapper]
internal static partial class UserMapper
{
    internal static partial UserDto ToDto(this User user);

    private static string MapEmail(Email email) => email.Value;
}