namespace Users.Infrastracture.Persistence;

internal sealed class UserEmailConflictException(Exception innerException)
    : Exception("A user with that email already exists.", innerException);

internal sealed class UserConcurrencyException(Exception innerException)
    : Exception("The user was changed by another request.", innerException);
