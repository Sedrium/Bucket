using Bucket.Common;
using Microsoft.AspNetCore.Http;

namespace Bucket.Api.Http;

public static class ResultFailureHttpStatus
{
    public static int GetStatusCode<T>(this Result<T> result) => result.FailureKind switch
    {
        ResultFailureKind.NotFound => StatusCodes.Status404NotFound,
        ResultFailureKind.BadRequest => StatusCodes.Status400BadRequest,
        ResultFailureKind.None => StatusCodes.Status500InternalServerError,
        _ => StatusCodes.Status400BadRequest,
    };
}
