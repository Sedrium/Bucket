namespace Bucket.Contract.Requests.Purchases;

public record AddPurchaseRequest(long CustomerId, IReadOnlyList<long> ProductIds);
