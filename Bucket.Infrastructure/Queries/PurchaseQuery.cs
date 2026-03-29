using Bucket.Application.Queries;
using Bucket.Common;
using Bucket.Contract;
using Bucket.Contract.Dtos.Purchases;
using Bucket.Domain.Purchases;
using DataModel = Bucket.Infrastructure.Data.Data;

namespace Bucket.Infrastructure.Queries;

public class PurchaseQuery : IPurchaseQuery
{
    private readonly DataModel _data;

    public PurchaseQuery(DataModel data)
    {
        _data = data;
    }

    public Task<Result<PagedResponse<PurchaseDTO>>> GetPurchasesAsync(Pagination pagination, CancellationToken cancellationToken)
    {
        var active = _data.Purchases.Where(p => !p.IsDeleted).OrderBy(p => p.Id).ToList();
        var totalCount = active.Count;

        var items = active
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(MapToDto)
            .ToList();

        return Task.FromResult(Result<PagedResponse<PurchaseDTO>>.Success(new PagedResponse<PurchaseDTO>(items, totalCount)));
    }

    public Task<PurchaseDTO?> GetPurchaseByIdAsync(long id, CancellationToken cancellationToken)
    {
        var purchase = _data.Purchases.FirstOrDefault(p => p.Id == id && !p.IsDeleted);
        return Task.FromResult(purchase is null ? null : MapToDto(purchase));
    }

    private static PurchaseDTO MapToDto(Purchase purchase)
    {
        if (!purchase.Id.HasValue)
        {
            throw new InvalidOperationException("Purchase id is required for mapping.");
        }

        return new PurchaseDTO(purchase.Id.Value, purchase.CustomerId, purchase.ProductIds);
    }
}
