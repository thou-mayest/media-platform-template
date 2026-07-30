using Users.Common;
using System.ComponentModel.DataAnnotations;

namespace Users.Presentation.Users;

public sealed record CreateUserRequest(
    [property: Required, StringLength(200, MinimumLength = 1)] string? Name,
    [property: Required, StringLength(256), EmailAddress] string? Email,
    [property: Required, StringLength(128, MinimumLength = 8)] string? Password,
    [property: Required, EnumDataType(typeof(Role))] Role? Role);

public sealed record UpdateUserRequest(
    [property: Required, StringLength(200, MinimumLength = 1)] string? Name,
    [property: Required, StringLength(256), EmailAddress] string? Email,
    [property: StringLength(128, MinimumLength = 8)] string? Password,
    [property: EnumDataType(typeof(Role))] Role? Role);
