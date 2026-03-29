using Bucket.Common;
using Bucket.Domain.Purchases;

namespace Bucket.Application.Interfaces;

public interface IPurchaseRepository
{
    Task<Purchase?> GetByIdAsync(long id, CancellationToken cancellationToken);

    Task<IReadOnlyList<Purchase>> GetActiveByCustomerIdAsync(long customerId, CancellationToken cancellationToken);

    Task<Result<long>> AddPurchaseAsync(Purchase purchase, CancellationToken cancellationToken);

    Task<Result<long>> UpdatePurchaseAsync(Purchase purchase, CancellationToken cancellationToken);
}
