using Bucket.Common;
using Bucket.Contract;
using Bucket.Contract.Dtos.Products;

namespace Bucket.Application.Queries;

public interface IProductQuery
{
    Task<Result<PagedResponse<ProductDTO>>> GetProductsAsync(Pagination pagination, CancellationToken cancellationToken);

    Task<ProductDTO?> GetProductByIdAsync(long id, CancellationToken cancellationToken);

    Task<IReadOnlyList<ProductDTO>> GetProductsByIdsAsync(
        IReadOnlyCollection<long> productIds,
        CancellationToken cancellationToken);
}
