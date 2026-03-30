using Bucket.Application.Handlers.Queries.Reports;
using Bucket.Application.Queries;
using Bucket.Application.Reports;
using Bucket.Application.Services;
using Bucket.Common;
using Bucket.Contract.Dtos.Persons;
using Bucket.Contract.Dtos.Products;
using Bucket.Contract.Dtos.Purchases;
using NSubstitute;
using Xunit;

namespace Bucket.Application.Tests.Handlers;

public class GetPurchaseCsvReportQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenPurchaseMissing_ReturnsNotFound()
    {
        // Given
        var purchaseQuery = Substitute.For<IPurchaseQuery>();
        purchaseQuery.GetPurchaseByIdAsync(1, Arg.Any<CancellationToken>()).Returns(Task.FromResult<PurchaseDTO?>(null));
        var subject = new GetPurchaseCsvReportQueryHandler(
            purchaseQuery,
            Substitute.For<IPersonQuery>(),
            Substitute.For<IProductQuery>(),
            Substitute.For<IRaportGenerator>());

        // When
        var result = await subject.Handle(new GetPurchaseCsvReportQuery(1), CancellationToken.None);

        // Then
        Assert.Equal(ResultFailureKind.NotFound, result.FailureKind);
    }

    [Fact]
    public async Task Handle_WhenCustomerMissing_ReturnsNotFound()
    {
        // Given
        var purchase = new PurchaseDTO(9, 1, new List<long> { 2 });
        var purchaseQuery = Substitute.For<IPurchaseQuery>();
        purchaseQuery.GetPurchaseByIdAsync(9, Arg.Any<CancellationToken>()).Returns(Task.FromResult<PurchaseDTO?>(purchase));
        var personQuery = Substitute.For<IPersonQuery>();
        personQuery.GetPersonByIdAsync(1, Arg.Any<CancellationToken>()).Returns(Task.FromResult<PersonDTO?>(null));
        var subject = new GetPurchaseCsvReportQueryHandler(
            purchaseQuery,
            personQuery,
            Substitute.For<IProductQuery>(),
            Substitute.For<IRaportGenerator>());

        // When
        var result = await subject.Handle(new GetPurchaseCsvReportQuery(9), CancellationToken.None);

        // Then
        Assert.Equal(ResultFailureKind.NotFound, result.FailureKind);
    }

    [Fact]
    public async Task Handle_WhenValid_ReturnsReport_AndBuildsInput()
    {
        // Given
        var purchase = new PurchaseDTO(9, 1, new List<long> { 2, 2 });
        var purchaseQuery = Substitute.For<IPurchaseQuery>();
        purchaseQuery.GetPurchaseByIdAsync(9, Arg.Any<CancellationToken>()).Returns(Task.FromResult<PurchaseDTO?>(purchase));

        var personQuery = Substitute.For<IPersonQuery>();
        personQuery.GetPersonByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<PersonDTO?>(new PersonDTO(1, "John", "Doe", 1990)));

        var productDto = new ProductDTO(2, "P", "T", 4.5);
        var productQuery = Substitute.For<IProductQuery>();
        productQuery.GetProductsByIdsAsync(Arg.Any<IReadOnlyCollection<long>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<ProductDTO>>(new List<ProductDTO> { productDto }));

        var report = new PurchaseCsvReport("f.csv", "content");
        var generator = Substitute.For<IRaportGenerator>();
        generator.GeneratePurchaseCsvReport(Arg.Any<PurchaseCsvReportInput>()).Returns(report);

        var subject = new GetPurchaseCsvReportQueryHandler(purchaseQuery, personQuery, productQuery, generator);

        // When
        var result = await subject.Handle(new GetPurchaseCsvReportQuery(9), CancellationToken.None);

        // Then
        Assert.True(result.IsSuccess);
        Assert.Same(report, result.Value);
        generator.Received(1).GeneratePurchaseCsvReport(
            Arg.Is<PurchaseCsvReportInput>(i => i.PurchaseId == 9 && i.CustomerDisplayName == "John Doe"));
    }
}
