using Bucket.Application.Repositories;
using Bucket.Common;
using Bucket.Domain.Purchases;
using DataModel = Bucket.Infrastructure.Data.Data;

namespace Bucket.Infrastructure.Repositories;

public class PurchaseRepository : IPurchaseRepository
{
    private readonly DataModel _data;

    public PurchaseRepository(DataModel data)
    {
        _data = data;
    }

    public Task<Purchase?> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        var purchase = _data.Purchases.FirstOrDefault(p => p.Id == id && !p.IsDeleted);
        return Task.FromResult(purchase);
    }

    public Task<IReadOnlyList<Purchase>> GetActiveByCustomerIdAsync(long customerId, CancellationToken cancellationToken)
    {
        var list = _data.Purchases.Where(p => p.CustomerId == customerId && !p.IsDeleted).ToList();
        IReadOnlyList<Purchase> result = list;
        return Task.FromResult(result);
    }

    public Task<Result<long>> AddPurchaseAsync(Purchase purchase, CancellationToken cancellationToken)
    {
        var id = GetNextId();

        purchase.SetId(id);

        _data.Purchases.Add(purchase);

        return Task.FromResult(Result<long>.Success(id));
    }

    public Task<Result> UpdatePurchaseAsync(Purchase purchase, CancellationToken cancellationToken)
    {
        if (!purchase.Id.HasValue)
        {
            return Task.FromResult(Result.Fail("Purchase id is required."));
        }

        var index = _data.Purchases.FindIndex(p => p.Id == purchase.Id);
        if (index < 0)
        {
            return Task.FromResult(Result.NotFound("Purchase not found."));
        }

        _data.Purchases[index] = purchase;

        return Task.FromResult(Result.Ok());
    }

    private long GetNextId()
    {
        if (_data.Purchases.Count == 0)
        {
            return 1;
        }

        return _data.Purchases.Max(p => p.Id!.Value) + 1;
    }
}
