using Bucket.Application.Queries;
using Bucket.Common;
using Bucket.Contract;
using Bucket.Contract.Dtos.Purchases;
using MediatR;

namespace Bucket.Application.Handlers.Queries.Purchases;

public record GetPurchasesQuery(Pagination Pagination) : IRequest<Result<PagedResponse<PurchaseDTO>>>;

public class GetPurchasesQueryHandler : IRequestHandler<GetPurchasesQuery, Result<PagedResponse<PurchaseDTO>>>
{
    private readonly IPurchaseQuery _purchaseQuery;

    public GetPurchasesQueryHandler(IPurchaseQuery purchaseQuery)
    {
        _purchaseQuery = purchaseQuery;
    }

    public Task<Result<PagedResponse<PurchaseDTO>>> Handle(GetPurchasesQuery request, CancellationToken cancellationToken) =>
        _purchaseQuery.GetPurchasesAsync(request.Pagination, cancellationToken);
}
