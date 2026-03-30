using Bucket.Application.Handlers.Queries.Purchases;
using Bucket.Application.Queries;
using Bucket.Common;
using Bucket.Contract;
using Bucket.Contract.Dtos.Purchases;
using NSubstitute;
using Xunit;

namespace Bucket.Application.Tests.Handlers;

public class GetPurchasesQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenQueryReturnsSuccess_ReturnsSameResult_AndDelegates()
    {
        // Given
        var pagination = Pagination.Create(1, 10).Value!;
        var expected = Result<PagedResponse<PurchaseDTO>>.Success(
            new PagedResponse<PurchaseDTO>(Array.Empty<PurchaseDTO>(), 0));
        var query = Substitute.For<IPurchaseQuery>();
        query.GetPurchasesAsync(pagination, Arg.Any<CancellationToken>()).Returns(Task.FromResult(expected));
        var subject = new GetPurchasesQueryHandler(query);

        // When
        var result = await subject.Handle(new GetPurchasesQuery(pagination), CancellationToken.None);

        // Then
        Assert.True(result.IsSuccess);
        Assert.Same(expected, result);
        await query.Received(1).GetPurchasesAsync(pagination, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenQueryReturnsFailure_ReturnsThatFailure_AndDelegates()
    {
        // Given
        var pagination = Pagination.Create(3, 15).Value!;
        var expected = Result<PagedResponse<PurchaseDTO>>.Failure("bad");
        var query = Substitute.For<IPurchaseQuery>();
        query.GetPurchasesAsync(pagination, Arg.Any<CancellationToken>()).Returns(Task.FromResult(expected));
        var subject = new GetPurchasesQueryHandler(query);

        // When
        var result = await subject.Handle(new GetPurchasesQuery(pagination), CancellationToken.None);

        // Then
        Assert.False(result.IsSuccess);
        Assert.Equal(ResultFailureKind.BadRequest, result.FailureKind);
        Assert.Same(expected, result);
        await query.Received(1).GetPurchasesAsync(pagination, Arg.Any<CancellationToken>());
    }
}
