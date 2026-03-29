namespace Bucket.Contract.Dtos.Purchases;

public record PurchaseDTO(long Id, long CustomerId, IReadOnlyList<long> ProductIds);
