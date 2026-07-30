namespace Users.Application.Users.Exceptions;

internal sealed class UserNotFoundException(Guid id)
    : Exception($"User '{id}' was not found.");

internal sealed class EmailAlreadyExistsException(Exception? innerException = null)
    : Exception("A user with that email already exists.", innerException);

internal sealed class InvalidCredentialsException()
    : Exception("The email or password is incorrect.");

internal sealed class UserConcurrencyException(Exception innerException)
    : Exception("The user was changed by another request. Reload it and try again.", innerException);
