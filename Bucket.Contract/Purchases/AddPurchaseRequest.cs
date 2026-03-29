namespace Bucket.Contract.Purchases;

public record AddPurchaseRequest(long CustomerId, IReadOnlyList<long> ProductIds);
