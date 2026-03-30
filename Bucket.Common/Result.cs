namespace Bucket.Common;

public enum ResultFailureKind
{
    None,
    BadRequest,
    NotFound,
}

public record Result(bool IsSuccess, string? Error, ResultFailureKind FailureKind)
{
    public static Result Ok() => new(true, null, ResultFailureKind.None);

    public static Result Fail(string error) => new(false, error, ResultFailureKind.BadRequest);

    public static Result NotFound(string error) => new(false, error, ResultFailureKind.NotFound);
}

public record Result<T>(bool IsSuccess, T? Value, string? Error, ResultFailureKind FailureKind = ResultFailureKind.None)
{
    public static Result<T> Success(T value) => new(true, value, null, ResultFailureKind.None);

    public static Result<T> Failure(string error) =>
        new(false, default, error, ResultFailureKind.BadRequest);

    public static Result<T> NotFound(string error) =>
        new(false, default, error, ResultFailureKind.NotFound);
}
