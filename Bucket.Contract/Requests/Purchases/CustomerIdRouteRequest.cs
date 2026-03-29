namespace Bucket.Contract.Requests.Purchases;

public record CustomerIdRouteRequest
{
    public long CustomerId { get; init; }
}
