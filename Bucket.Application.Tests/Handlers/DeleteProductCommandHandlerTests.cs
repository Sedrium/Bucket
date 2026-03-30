using Bucket.Application.Handlers.Commands.Products;
using Bucket.Application.Repositories;
using Bucket.Application.Tests.TestHelpers;
using Bucket.Common;
using Bucket.Domain.Products;
using NSubstitute;
using Xunit;

namespace Bucket.Application.Tests.Handlers;

public class DeleteProductCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenMissing_ReturnsNotFound()
    {
        // Given
        var repo = Substitute.For<IProductRepository>();
        repo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(Task.FromResult<Product?>(null));
        var subject = new DeleteProductCommandHandler(repo);

        // When
        var result = await subject.Handle(new DeleteProductCommand(1), CancellationToken.None);

        // Then
        Assert.Equal(ResultFailureKind.NotFound, result.FailureKind);
        await repo.DidNotReceive().UpdateProductAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenProductHasNoIdentity_ReturnsBadRequest()
    {
        // Given
        var productWithoutId = Product.Create("N", "T", 1).Value!;
        var repo = Substitute.For<IProductRepository>();
        repo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(Task.FromResult<Product?>(productWithoutId));
        var subject = new DeleteProductCommandHandler(repo);

        // When
        var result = await subject.Handle(new DeleteProductCommand(1), CancellationToken.None);

        // Then
        Assert.False(result.IsSuccess);
        Assert.Equal(ResultFailureKind.BadRequest, result.FailureKind);
        await repo.DidNotReceive().UpdateProductAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenAlreadyDeleted_ReturnsBadRequest()
    {
        // Given
        var product = DomainBuilders.ProductWithId(1);
        Assert.True(product.Delete().IsSuccess);
        var repo = Substitute.For<IProductRepository>();
        repo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(Task.FromResult<Product?>(product));
        var subject = new DeleteProductCommandHandler(repo);

        // When
        var result = await subject.Handle(new DeleteProductCommand(1), CancellationToken.None);

        // Then
        Assert.False(result.IsSuccess);
        Assert.Equal(ResultFailureKind.BadRequest, result.FailureKind);
        await repo.DidNotReceive().UpdateProductAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenRepositoryUpdateFails_ReturnsThatFailure()
    {
        // Given
        var product = DomainBuilders.ProductWithId(1);
        var repo = Substitute.For<IProductRepository>();
        repo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(Task.FromResult<Product?>(product));
        repo.UpdateProductAsync(product, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Fail("error")));
        var subject = new DeleteProductCommandHandler(repo);

        // When
        var result = await subject.Handle(new DeleteProductCommand(1), CancellationToken.None);

        // Then
        Assert.False(result.IsSuccess);
        Assert.Equal(ResultFailureKind.BadRequest, result.FailureKind);
    }

    [Fact]
    public async Task Handle_WhenValid_ReturnsOk()
    {
        // Given
        var product = DomainBuilders.ProductWithId(1);
        var repo = Substitute.For<IProductRepository>();
        repo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(Task.FromResult<Product?>(product));
        repo.UpdateProductAsync(product, Arg.Any<CancellationToken>()).Returns(Task.FromResult(Result.Ok()));
        var subject = new DeleteProductCommandHandler(repo);

        // When
        var result = await subject.Handle(new DeleteProductCommand(1), CancellationToken.None);

        // Then
        Assert.True(result.IsSuccess);
        await repo.Received(1).UpdateProductAsync(product, Arg.Any<CancellationToken>());
    }
}
