using Bucket.Common;

namespace Bucket.Contract;

public sealed class Pagination
{
    public const int MaxPageSize = 100;

    public int Page { get; }
    public int PageSize { get; }

    private Pagination(int page, int pageSize)
    {
        Page = page;
        PageSize = pageSize;
    }

    public static Result<Pagination> Create(int page, int pageSize)
    {
        if (page < 1)
        {
            return Result<Pagination>.Failure("Page must be at least 1.");
        }

        if (pageSize < 1)
        {
            return Result<Pagination>.Failure("PageSize must be at least 1.");
        }

        if (pageSize > MaxPageSize)
        {
            return Result<Pagination>.Failure($"PageSize cannot be greater than {MaxPageSize}.");
        }

        return Result<Pagination>.Success(new Pagination(page, pageSize));
    }
}
