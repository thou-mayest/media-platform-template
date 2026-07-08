namespace SharedKernal.Results
{
    public sealed record Error(string Code, string Message, ErrorType Type = ErrorType.Failure)
    {
        public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Failure);

        public static Error Validation(string code, string message) => new(code, message, ErrorType.Validation);
        public static Error NotFound(string code, string message) => new(code, message, ErrorType.NotFound);
        public static Error Conflict(string code, string message) => new(code, message, ErrorType.Conflict);
        public static Error Failure(string code, string message) => new(code, message, ErrorType.Failure);
    }

    public enum ErrorType
    {
        Failure = 0,
        Validation = 1,
        NotFound = 2,
        Conflict = 3
    }

    public static class ErrorCodes
    {
        public const string NotFound = "NotFound";
        public const string Validation = "Validation";
        public const string Conflict = "Conflict";
        public const string Failure = "Failure";
    }
}