namespace Users.Application.Abstractions;

internal sealed class DuplicateUserEmailException : Exception
{
    public DuplicateUserEmailException(Exception innerException)
        : base("A user with that email already exists.", innerException)
    {
    }
}
