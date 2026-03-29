using Bucket.Application.Queries;
using Bucket.Application.Reports;
using Bucket.Application.Services;
using Bucket.Common;
using Bucket.Domain.Products;
using MediatR;

namespace Bucket.Application.Handlers.Queries.Reports;

public record GetPurchaseCsvReportQuery(long PurchaseId) : IRequest<Result<PurchaseCsvReport>>;

public class GetPurchaseCsvReportQueryHandler : IRequestHandler<GetPurchaseCsvReportQuery, Result<PurchaseCsvReport>>
{
    private readonly IPurchaseQuery _purchaseQuery;
    private readonly IPersonQuery _personQuery;
    private readonly IProductQuery _productQuery;
    private readonly IRaportGenerator _raportGenerator;

    public GetPurchaseCsvReportQueryHandler(
        IPurchaseQuery purchaseQuery,
        IPersonQuery personQuery,
        IProductQuery productQuery,
        IRaportGenerator raportGenerator)
    {
        _purchaseQuery = purchaseQuery;
        _personQuery = personQuery;
        _productQuery = productQuery;
        _raportGenerator = raportGenerator;
    }

    public async Task<Result<PurchaseCsvReport>> Handle(GetPurchaseCsvReportQuery request, CancellationToken cancellationToken)
    {
        var purchase = await _purchaseQuery.GetPurchaseByIdAsync(request.PurchaseId, cancellationToken);
        if (purchase is null)
        {
            return Result<PurchaseCsvReport>.NotFound("Purchase not found.");
        }

        var person = await _personQuery.GetPersonByIdAsync(purchase.CustomerId, cancellationToken);
        if (person is null)
        {
            return Result<PurchaseCsvReport>.NotFound("Customer not found.");
        }

        var counts = purchase.ProductIds
            .GroupBy(id => id)
            .OrderBy(g => g.Key)
            .Select(g => (ProductId: g.Key, Count: g.Count()))
            .ToList();

        var distinctIds = counts.ConvertAll(c => c.ProductId);

        var products = await _productQuery.GetProductsByIdsAsync(distinctIds, cancellationToken);

        var lines = products.GroupJoin(purchase.ProductIds, x => x.Id, x => x, (x, y) =>
        {
            return new PurchaseCsvLineItem(x.Id, y.Count(), x.Name, x.Price);
        });

        var customerDisplayName = $"{person.Firstname} {person.Lastname}".Trim();

        var input = new PurchaseCsvReportInput(purchase.Id, customerDisplayName, lines.ToList());

        var report = _raportGenerator.GeneratePurchaseCsvReport(input);

        return Result<PurchaseCsvReport>.Success(report);
    }
}
