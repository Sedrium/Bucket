using Bucket.Application.Handlers.Commands.Products;
using Bucket.Application.Repositories;
using Bucket.Application.Tests.TestHelpers;
using Bucket.Common;
using Bucket.Domain.Products;
using NSubstitute;
using Xunit;

namespace Bucket.Application.Tests.Handlers;

public class UpdateProductCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenProductMissing_ReturnsNotFound()
    {
        // Given
        var repo = Substitute.For<IProductRepository>();
        repo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(Task.FromResult<Product?>(null));
        var subject = new UpdateProductCommandHandler(repo);

        // When
        var result = await subject.Handle(new UpdateProductCommand(1, "A", "B", 2), CancellationToken.None);

        // Then
        Assert.Equal(ResultFailureKind.NotFound, result.FailureKind);
        await repo.DidNotReceive().UpdateProductAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenDomainUpdateInvalid_ReturnsBadRequest()
    {
        // Given
        var product = DomainBuilders.ProductWithId(1);
        var repo = Substitute.For<IProductRepository>();
        repo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(Task.FromResult<Product?>(product));
        var subject = new UpdateProductCommandHandler(repo);

        // When — empty type
        var result = await subject.Handle(new UpdateProductCommand(1, "X", "  ", 3), CancellationToken.None);

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
            .Returns(Task.FromResult(Result.NotFound("gone")));
        var subject = new UpdateProductCommandHandler(repo);

        // When
        var result = await subject.Handle(new UpdateProductCommand(1, "X", "Y", 3), CancellationToken.None);

        // Then
        Assert.False(result.IsSuccess);
        Assert.Equal(ResultFailureKind.NotFound, result.FailureKind);
    }

    [Fact]
    public async Task Handle_WhenValid_ReturnsOk()
    {
        // Given
        var product = DomainBuilders.ProductWithId(1);
        var repo = Substitute.For<IProductRepository>();
        repo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(Task.FromResult<Product?>(product));
        repo.UpdateProductAsync(product, Arg.Any<CancellationToken>()).Returns(Task.FromResult(Result.Ok()));
        var subject = new UpdateProductCommandHandler(repo);

        // When
        var result = await subject.Handle(new UpdateProductCommand(1, "X", "Y", 3), CancellationToken.None);

        // Then
        Assert.True(result.IsSuccess);
        await repo.Received(1).UpdateProductAsync(product, Arg.Any<CancellationToken>());
    }
}
