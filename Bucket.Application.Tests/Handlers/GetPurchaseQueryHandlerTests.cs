using Bucket.Application.Handlers.Queries.Purchases;
using Bucket.Application.Queries;
using Bucket.Common;
using Bucket.Contract.Dtos.Purchases;
using NSubstitute;
using Xunit;

namespace Bucket.Application.Tests.Handlers;

public class GetPurchaseQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenFound_ReturnsSuccess()
    {
        // Given
        var query = Substitute.For<IPurchaseQuery>();
        var dto = new PurchaseDTO(1, 10, new List<long> { 2 });
        query.GetPurchaseByIdAsync(1, Arg.Any<CancellationToken>()).Returns(Task.FromResult<PurchaseDTO?>(dto));
        var subject = new GetPurchaseQueryHandler(query);

        // When
        var result = await subject.Handle(new GetPurchaseQuery(1), CancellationToken.None);

        // Then
        Assert.True(result.IsSuccess);
        Assert.Equal(dto, result.Value);
    }

    [Fact]
    public async Task Handle_WhenMissing_ReturnsNotFound()
    {
        // Given
        var query = Substitute.For<IPurchaseQuery>();
        query.GetPurchaseByIdAsync(1, Arg.Any<CancellationToken>()).Returns(Task.FromResult<PurchaseDTO?>(null));
        var subject = new GetPurchaseQueryHandler(query);

        // When
        var result = await subject.Handle(new GetPurchaseQuery(1), CancellationToken.None);

        // Then
        Assert.Equal(ResultFailureKind.NotFound, result.FailureKind);
    }
}
