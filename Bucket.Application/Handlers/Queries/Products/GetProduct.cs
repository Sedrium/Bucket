using Bucket.Application.Interfaces;
using Bucket.Common;
using Bucket.Contract.Dtos.Products;
using MediatR;

namespace Bucket.Application.Handlers.Queries.Products;

public record GetProductQuery(long Id) : IRequest<Result<ProductDTO>>;

public class GetProductQueryHandler : IRequestHandler<GetProductQuery, Result<ProductDTO>>
{
    private readonly IProductQuery _productQuery;

    public GetProductQueryHandler(IProductQuery productQuery)
    {
        _productQuery = productQuery;
    }

    public async Task<Result<ProductDTO>> Handle(GetProductQuery request, CancellationToken cancellationToken)
    {
        var dto = await _productQuery.GetProductByIdAsync(request.Id, cancellationToken);

        if (dto is null)
        {
            return Result<ProductDTO>.Failure("Product not found.");
        }

        return Result<ProductDTO>.Success(dto);
    }
}
