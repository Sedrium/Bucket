using Bucket.Common;

namespace Bucket.Domain.Purchases;

public class Purchase
{
    private const int MaxProductLines = 100;

    public long? Id { get; private set; }
    public long CustomerId { get; private set; }
    public IReadOnlyList<long> ProductIds { get; private set; }
    public bool IsDeleted { get; private set; }

    private Purchase(long customerId, List<long> productIds)
    {
        CustomerId = customerId;
        ProductIds = productIds;
    }

    public static Result<Purchase> Create(long customerId, IReadOnlyList<long>? productIds)
    {
        if (customerId < 1)
        {
            return Result<Purchase>.Failure("Customer id must be at least 1.");
        }

        if (productIds is null || productIds.Count == 0)
        {
            return Result<Purchase>.Failure("At least one product is required.");
        }

        if (productIds.Count > MaxProductLines)
        {
            return Result<Purchase>.Failure($"A purchase cannot contain more than {MaxProductLines} products.");
        }

        foreach (var pid in productIds)
        {
            if (pid < 1)
            {
                return Result<Purchase>.Failure("Each product id must be at least 1.");
            }
        }

        return Result<Purchase>.Success(new Purchase(customerId, productIds.ToList()));
    }

    public Result Delete()
    {
        if (!Id.HasValue)
        {
            return Result.Fail("Purchase has no identity.");
        }

        if (IsDeleted)
        {
            return Result.Fail("Purchase is already deleted.");
        }

        IsDeleted = true;

        return Result.Ok();
    }

    public void SetId(long id)
    {
        if (!Id.HasValue)
        {
            Id = id;
        }
    }
}
