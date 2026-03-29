using Bucket.Common;
using Bucket.Domain.Persons;
using Bucket.Domain.Products;

namespace Bucket.Domain.Purchases;

public static class PurchaseComposition
{
    public static Result<Purchase> TryCreate(
        Person customer,
        IReadOnlyList<long> productIds,
        IReadOnlyList<Product> resolvedProducts)
    {
        if (!customer.Id.HasValue)
        {
            return Result<Purchase>.Failure("Customer has no identity.");
        }

        var catalog = resolvedProducts
            .Where(p => p.Id.HasValue && !p.IsDeleted)
            .ToDictionary(p => p.Id!.Value);

        foreach (var productId in productIds.Distinct())
        {
            if (!catalog.ContainsKey(productId))
            {
                return Result<Purchase>.NotFound($"Product with id {productId} not found.");
            }
        }

        return Purchase.Create(customer.Id.Value, productIds);
    }
}
