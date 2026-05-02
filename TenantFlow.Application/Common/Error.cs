namespace TenantFlow.Application.Common;

public readonly struct Error
{
    public ErrorType ErrorType { get; }
    public string Message { get; }

    // Private constructor — callers must use the static factory methods below
    private Error(ErrorType errorType, string message)
    {
        ErrorType = errorType;
        Message = message;
    }

    // The "no error" sentinel — used as the Error value on successful Results
    public static readonly Error None = new(ErrorType.None, string.Empty);

    // Factory methods — one per ErrorType, so call sites are readable
    public static Error NotFound(string message) => new(ErrorType.NotFound, message);
    public static Error Unauthorized(string message) => new(ErrorType.Unauthorized, message);
    public static Error Conflict(string message) => new(ErrorType.Conflict, message);
    public static Error Validation(string message) => new(ErrorType.Validation, message);
}