using Bucket.Application.Handlers.Commands.Purchases;
using Bucket.Application.Repositories;
using Bucket.Application.Tests.TestHelpers;
using Bucket.Common;
using Bucket.Domain.Purchases;
using NSubstitute;
using Xunit;

namespace Bucket.Application.Tests.Handlers;

public class DeletePurchaseCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenMissing_ReturnsNotFound()
    {
        // Given
        var repo = Substitute.For<IPurchaseRepository>();
        repo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(Task.FromResult<Purchase?>(null));
        var subject = new DeletePurchaseCommandHandler(repo);

        // When
        var result = await subject.Handle(new DeletePurchaseCommand(1), CancellationToken.None);

        // Then
        Assert.Equal(ResultFailureKind.NotFound, result.FailureKind);
        await repo.DidNotReceive().UpdatePurchaseAsync(Arg.Any<Purchase>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenPurchaseHasNoIdentity_ReturnsBadRequest()
    {
        // Given
        var purchaseWithoutId = Purchase.Create(10, new List<long> { 2 }).Value!;
        var repo = Substitute.For<IPurchaseRepository>();
        repo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(Task.FromResult<Purchase?>(purchaseWithoutId));
        var subject = new DeletePurchaseCommandHandler(repo);

        // When
        var result = await subject.Handle(new DeletePurchaseCommand(1), CancellationToken.None);

        // Then
        Assert.False(result.IsSuccess);
        Assert.Equal(ResultFailureKind.BadRequest, result.FailureKind);
        await repo.DidNotReceive().UpdatePurchaseAsync(Arg.Any<Purchase>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenAlreadyDeleted_ReturnsBadRequest()
    {
        // Given
        var purchase = DomainBuilders.PurchaseWithId(1, 10, new List<long> { 2 });
        Assert.True(purchase.Delete().IsSuccess);
        var repo = Substitute.For<IPurchaseRepository>();
        repo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(Task.FromResult<Purchase?>(purchase));
        var subject = new DeletePurchaseCommandHandler(repo);

        // When
        var result = await subject.Handle(new DeletePurchaseCommand(1), CancellationToken.None);

        // Then
        Assert.False(result.IsSuccess);
        Assert.Equal(ResultFailureKind.BadRequest, result.FailureKind);
        await repo.DidNotReceive().UpdatePurchaseAsync(Arg.Any<Purchase>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenRepositoryUpdateFails_ReturnsThatFailure()
    {
        // Given
        var purchase = DomainBuilders.PurchaseWithId(1, 10, new List<long> { 2 });
        var repo = Substitute.For<IPurchaseRepository>();
        repo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(Task.FromResult<Purchase?>(purchase));
        repo.UpdatePurchaseAsync(purchase, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Fail("error")));
        var subject = new DeletePurchaseCommandHandler(repo);

        // When
        var result = await subject.Handle(new DeletePurchaseCommand(1), CancellationToken.None);

        // Then
        Assert.False(result.IsSuccess);
        Assert.Equal(ResultFailureKind.BadRequest, result.FailureKind);
    }

    [Fact]
    public async Task Handle_WhenValid_ReturnsOk()
    {
        // Given
        var purchase = DomainBuilders.PurchaseWithId(1, 10, new List<long> { 2 });
        var repo = Substitute.For<IPurchaseRepository>();
        repo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(Task.FromResult<Purchase?>(purchase));
        repo.UpdatePurchaseAsync(purchase, Arg.Any<CancellationToken>()).Returns(Task.FromResult(Result.Ok()));
        var subject = new DeletePurchaseCommandHandler(repo);

        // When
        var result = await subject.Handle(new DeletePurchaseCommand(1), CancellationToken.None);

        // Then
        Assert.True(result.IsSuccess);
        await repo.Received(1).UpdatePurchaseAsync(purchase, Arg.Any<CancellationToken>());
    }
}
