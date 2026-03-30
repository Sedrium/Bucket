using Bucket.Application.Handlers.Queries.Products;
using Bucket.Application.Queries;
using Bucket.Common;
using Bucket.Contract;
using Bucket.Contract.Dtos.Products;
using NSubstitute;
using Xunit;

namespace Bucket.Application.Tests.Handlers;

public class GetProductsQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenQueryReturnsSuccess_ReturnsSameResult_AndDelegates()
    {
        // Given
        var pagination = Pagination.Create(1, 10).Value!;
        var expected = Result<PagedResponse<ProductDTO>>.Success(
            new PagedResponse<ProductDTO>(Array.Empty<ProductDTO>(), 0));
        var query = Substitute.For<IProductQuery>();
        query.GetProductsAsync(pagination, Arg.Any<CancellationToken>()).Returns(Task.FromResult(expected));
        var subject = new GetProductsQueryHandler(query);

        // When
        var result = await subject.Handle(new GetProductsQuery(pagination), CancellationToken.None);

        // Then
        Assert.True(result.IsSuccess);
        Assert.Same(expected, result);
        await query.Received(1).GetProductsAsync(pagination, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenQueryReturnsFailure_ReturnsThatFailure_AndDelegates()
    {
        // Given
        var pagination = Pagination.Create(1, 20).Value!;
        var expected = Result<PagedResponse<ProductDTO>>.NotFound("none");
        var query = Substitute.For<IProductQuery>();
        query.GetProductsAsync(pagination, Arg.Any<CancellationToken>()).Returns(Task.FromResult(expected));
        var subject = new GetProductsQueryHandler(query);

        // When
        var result = await subject.Handle(new GetProductsQuery(pagination), CancellationToken.None);

        // Then
        Assert.False(result.IsSuccess);
        Assert.Equal(ResultFailureKind.NotFound, result.FailureKind);
        Assert.Same(expected, result);
        await query.Received(1).GetProductsAsync(pagination, Arg.Any<CancellationToken>());
    }
}
