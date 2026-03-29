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
        var active = _data.Products.Where(p => !p.IsDeleted).OrderBy(p => p.Id).ToList();
        var totalCount = active.Count;

        var items = active
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(MapToDto)
            .ToList();

        return Task.FromResult(Result<PagedResponse<ProductDTO>>.Success(new PagedResponse<ProductDTO>(items, totalCount)));
    }

    public Task<ProductDTO?> GetProductByIdAsync(long id, CancellationToken cancellationToken)
    {
        var product = _data.Products.FirstOrDefault(p => p.Id == id && !p.IsDeleted);
        return Task.FromResult(product is null ? null : MapToDto(product));
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
