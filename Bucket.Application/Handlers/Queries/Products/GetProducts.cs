using Bucket.Application.Queries;
using Bucket.Common;
using Bucket.Contract;
using Bucket.Contract.Dtos.Products;
using MediatR;

namespace Bucket.Application.Handlers.Queries.Products;

public record GetProductsQuery(Pagination Pagination) : IRequest<Result<PagedResponse<ProductDTO>>>;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, Result<PagedResponse<ProductDTO>>>
{
    private readonly IProductQuery _productQuery;

    public GetProductsQueryHandler(IProductQuery productQuery)
    {
        _productQuery = productQuery;
    }

    public Task<Result<PagedResponse<ProductDTO>>> Handle(GetProductsQuery request, CancellationToken cancellationToken) =>
        _productQuery.GetProductsAsync(request.Pagination, cancellationToken);
}
