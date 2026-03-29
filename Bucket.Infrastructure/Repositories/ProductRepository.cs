using Bucket.Application.Repositories;
using Bucket.Common;
using Bucket.Domain.Products;
using DataModel = Bucket.Infrastructure.Data.Data;

namespace Bucket.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly DataModel _data;

    public ProductRepository(DataModel data)
    {
        _data = data;
    }

    public Task<Product?> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        var product = _data.Products.FirstOrDefault(p => p.Id == id && !p.IsDeleted);
        return Task.FromResult(product);
    }

    public Task<IReadOnlyList<Product>> GetByIdsAsync(IReadOnlyCollection<long> productIds, CancellationToken cancellationToken)
    {
        var list = _data.Products
            .Where(p => !p.IsDeleted && p.Id.HasValue && productIds.Contains(p.Id.Value))
            .ToList();

        return Task.FromResult<IReadOnlyList<Product>>(list);
    }

    public Task<Result<long>> AddProductAsync(Product product, CancellationToken cancellationToken)
    {
        var id = GetNextId();

        product.SetId(id);

        _data.Products.Add(product);

        return Task.FromResult(Result<long>.Success(id));
    }

    public Task<Result> UpdateProductAsync(Product product, CancellationToken cancellationToken)
    {
        if (!product.Id.HasValue)
        {
            return Task.FromResult(Result.Fail("Product id is required."));
        }

        var index = _data.Products.FindIndex(p => p.Id == product.Id);
        if (index < 0)
        {
            return Task.FromResult(Result.NotFound("Product not found."));
        }

        _data.Products[index] = product;

        return Task.FromResult(Result.Ok());
    }

    private long GetNextId()
    {
        if (_data.Products.Count == 0)
        {
            return 1;
        }

        return _data.Products.Max(p => p.Id!.Value) + 1;
    }
}
