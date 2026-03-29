using Bucket.Common;
using Bucket.Domain.Products;

namespace Bucket.Application.Interfaces;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(long id, CancellationToken cancellationToken);

    Task<Result<long>> AddProductAsync(Product product, CancellationToken cancellationToken);

    Task<Result<long>> UpdateProductAsync(Product product, CancellationToken cancellationToken);
}
