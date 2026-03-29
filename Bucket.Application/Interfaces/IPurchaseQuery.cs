using Bucket.Common;
using Bucket.Contract;
using Bucket.Contract.Dtos.Purchases;

namespace Bucket.Application.Interfaces;

public interface IPurchaseQuery
{
    Task<Result<PagedResponse<PurchaseDTO>>> GetPurchasesAsync(Pagination pagination, CancellationToken cancellationToken);

    Task<PurchaseDTO?> GetPurchaseByIdAsync(long id, CancellationToken cancellationToken);
}
