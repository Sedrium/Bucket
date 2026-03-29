namespace Bucket.Contract;

public record PagedResponse<T>(IEnumerable<T> Items, int TotalCount);
