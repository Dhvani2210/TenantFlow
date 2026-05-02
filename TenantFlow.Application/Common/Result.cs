namespace TenantFlow.Application.Common;

public class Result
{
    public bool IsSuccess { get; protected set; }
    public string? Error { get; protected set; }
    public ErrorType ErrorType { get; protected set; }

    protected Result() { }

    public static Result Success()
        => new Result { IsSuccess = true, ErrorType = ErrorType.None };

    public static Result Failure(string error, ErrorType errorType)
        => new Result { IsSuccess = false, Error = error, ErrorType = errorType };
}

public class Result<T> : Result
{
    public T? Value { get; private set; }

    private Result() { }

    public static Result<T> Success(T value)
        => new Result<T> { IsSuccess = true, Value = value, ErrorType = ErrorType.None };

    // 'new' here tells the compiler we are intentionally hiding
    // the parent's Failure method so we return Result<T> instead of Result
    public static new Result<T> Failure(string error, ErrorType errorType)
        => new Result<T> { IsSuccess = false, Error = error, ErrorType = errorType };
}