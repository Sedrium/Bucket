using Bucket.Application.Handlers.Commands.Products;
using Bucket.Application.Repositories;
using Bucket.Common;
using Bucket.Domain.Products;
using NSubstitute;
using Xunit;

namespace Bucket.Application.Tests.Handlers;

public class AddProductCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenProductValidationFails_ReturnsFailure_AndDoesNotCallRepository()
    {
        // Given
        var repo = Substitute.For<IProductRepository>();
        var subject = new AddProductCommandHandler(repo);

        // When — negative price
        var result = await subject.Handle(new AddProductCommand("N", "T", -1), CancellationToken.None);

        // Then
        Assert.False(result.IsSuccess);
        Assert.Equal(ResultFailureKind.BadRequest, result.FailureKind);
        await repo.DidNotReceive().AddProductAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenNameEmpty_ReturnsFailure()
    {
        // Given
        var repo = Substitute.For<IProductRepository>();
        var subject = new AddProductCommandHandler(repo);

        // When
        var result = await subject.Handle(new AddProductCommand("  ", "T", 1), CancellationToken.None);

        // Then
        Assert.False(result.IsSuccess);
        Assert.Equal(ResultFailureKind.BadRequest, result.FailureKind);
        await repo.DidNotReceive().AddProductAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenRepositoryReturnsNotFound_ReturnsNotFound()
    {
        // Given
        var repo = Substitute.For<IProductRepository>();
        repo.AddProductAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result<long>.NotFound("dup")));
        var subject = new AddProductCommandHandler(repo);

        // When
        var result = await subject.Handle(new AddProductCommand("Name", "Type", 3.5), CancellationToken.None);

        // Then
        Assert.False(result.IsSuccess);
        Assert.Equal(ResultFailureKind.NotFound, result.FailureKind);
    }

    [Fact]
    public async Task Handle_WhenRepositoryReturnsFailure_ReturnsBadRequest()
    {
        // Given
        var repo = Substitute.For<IProductRepository>();
        repo.AddProductAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result<long>.Failure("db")));
        var subject = new AddProductCommandHandler(repo);

        // When
        var result = await subject.Handle(new AddProductCommand("Name", "Type", 3.5), CancellationToken.None);

        // Then
        Assert.False(result.IsSuccess);
        Assert.Equal(ResultFailureKind.BadRequest, result.FailureKind);
    }

    [Fact]
    public async Task Handle_WhenValid_ReturnsEntityId()
    {
        // Given
        var repo = Substitute.For<IProductRepository>();
        repo.AddProductAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result<long>.Success(9L)));
        var subject = new AddProductCommandHandler(repo);

        // When
        var result = await subject.Handle(new AddProductCommand("Name", "Type", 3.5), CancellationToken.None);

        // Then
        Assert.True(result.IsSuccess);
        Assert.Equal(9L, result.Value!.Value);
    }
}
