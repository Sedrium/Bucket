using Bucket.Application.Interfaces;
using Bucket.Common;
using Bucket.Contract;
using Bucket.Contract.Dtos.Products;
using Bucket.Domain.Products;
using DataModel = Bucket.Infrastructure.Data.Data;

namespace Bucket.Infrastructure.Queries;

public class ProductQuery : IProductQuery
{
    private readonly DataModel _data;

    public ProductQuery(DataModel data)
    {
        _data = data;
    }

    public Task<Result<PagedResponse<ProductDTO>>> GetProductsAsync(Pagination pagination, CancellationToken cancellationToken)
    {
        var ordered = _data.Products.OrderBy(p => p.Id ?? long.MaxValue).ToList();
        var totalCount = ordered.Count;

        var items = ordered
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(MapToDto)
            .ToList();

        return Task.FromResult(Result<PagedResponse<ProductDTO>>.Success(new PagedResponse<ProductDTO>(items, totalCount)));
    }

    private static ProductDTO MapToDto(Product product)
    {
        if (!product.Id.HasValue)
        {
            throw new InvalidOperationException("Product id is required for mapping.");
        }

        return new ProductDTO(product.Id.Value, product.Name, product.Type, product.Price);
    }
}
