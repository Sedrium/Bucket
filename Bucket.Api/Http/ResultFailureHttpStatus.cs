using Bucket.Common;

namespace Bucket.Api.Http;

public static class ResultFailureHttpStatus
{
    public static int GetStatusCode(this Result result) => FromFailureKind(result.FailureKind);

    public static int GetStatusCode<T>(this Result<T> result) => FromFailureKind(result.FailureKind);

    private static int FromFailureKind(ResultFailureKind kind) => kind switch
    {
        ResultFailureKind.NotFound => StatusCodes.Status404NotFound,
        ResultFailureKind.BadRequest => StatusCodes.Status400BadRequest,
        ResultFailureKind.None => StatusCodes.Status500InternalServerError,
        _ => StatusCodes.Status400BadRequest,
    };
}
