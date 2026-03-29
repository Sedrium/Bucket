using Bucket.Application.Queries;
using Bucket.Common;
using Bucket.Contract.Dtos.Purchases;
using MediatR;

namespace Bucket.Application.Handlers.Queries.Purchases;

public record GetPurchaseQuery(long Id) : IRequest<Result<PurchaseDTO>>;

public class GetPurchaseQueryHandler : IRequestHandler<GetPurchaseQuery, Result<PurchaseDTO>>
{
    private readonly IPurchaseQuery _purchaseQuery;

    public GetPurchaseQueryHandler(IPurchaseQuery purchaseQuery)
    {
        _purchaseQuery = purchaseQuery;
    }

    public async Task<Result<PurchaseDTO>> Handle(GetPurchaseQuery request, CancellationToken cancellationToken)
    {
        var dto = await _purchaseQuery.GetPurchaseByIdAsync(request.Id, cancellationToken);

        if (dto is null)
        {
            return Result<PurchaseDTO>.NotFound("Purchase not found.");
        }

        return Result<PurchaseDTO>.Success(dto);
    }
}
