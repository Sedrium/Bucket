using Bucket.Application.Handlers.Queries.Products;
using Bucket.Application.Queries;
using Bucket.Common;
using Bucket.Contract.Dtos.Products;
using NSubstitute;
using Xunit;

namespace Bucket.Application.Tests.Handlers;

public class GetProductQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenFound_ReturnsSuccess()
    {
        // Given
        var query = Substitute.For<IProductQuery>();
        var dto = new ProductDTO(1, "N", "T", 1.5);
        query.GetProductByIdAsync(1, Arg.Any<CancellationToken>()).Returns(Task.FromResult<ProductDTO?>(dto));
        var subject = new GetProductQueryHandler(query);

        // When
        var result = await subject.Handle(new GetProductQuery(1), CancellationToken.None);

        // Then
        Assert.True(result.IsSuccess);
        Assert.Equal(dto, result.Value);
    }

    [Fact]
    public async Task Handle_WhenMissing_ReturnsNotFound()
    {
        // Given
        var query = Substitute.For<IProductQuery>();
        query.GetProductByIdAsync(1, Arg.Any<CancellationToken>()).Returns(Task.FromResult<ProductDTO?>(null));
        var subject = new GetProductQueryHandler(query);

        // When
        var result = await subject.Handle(new GetProductQuery(1), CancellationToken.None);

        // Then
        Assert.Equal(ResultFailureKind.NotFound, result.FailureKind);
    }
}
