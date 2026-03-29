namespace Bucket.Common;

public enum ResultFailureKind
{
    None,
    BadRequest,
    NotFound,
}

public readonly struct Result
{
    public bool IsSuccess { get; }
    public string? Error { get; }

    private Result(bool isSuccess, string? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Ok() => new(true, null);

    public static Result Fail(string error) => new(false, error);
}

public record Result<T>(bool IsSuccess, T? Value, string? Error, ResultFailureKind FailureKind = ResultFailureKind.None)
{
    public static Result<T> Success(T value) => new(true, value, null, ResultFailureKind.None);

    public static Result<T> Failure(string error) =>
        new(false, default, error, ResultFailureKind.BadRequest);

    public static Result<T> NotFound(string error) =>
        new(false, default, error, ResultFailureKind.NotFound);
}
