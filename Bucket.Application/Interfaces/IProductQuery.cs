using Bucket.Common;
using Bucket.Contract;
using Bucket.Contract.Dtos.Products;

namespace Bucket.Application.Interfaces;

public interface IProductQuery
{
    Task<Result<PagedResponse<ProductDTO>>> GetProductsAsync(Pagination pagination, CancellationToken cancellationToken);
}
