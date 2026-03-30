using Bucket.Application.Handlers.Commands.Purchases;
using Bucket.Application.Repositories;
using Bucket.Application.Tests.TestHelpers;
using Bucket.Common;
using Bucket.Domain.Purchases;
using NSubstitute;
using Xunit;

namespace Bucket.Application.Tests.Handlers;

public class DeletePurchasesByCustomerCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenCustomerMissing_ReturnsNotFound()
    {
        // Given
        var personRepo = Substitute.For<IPersonRepository>();
        personRepo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(Task.FromResult<Domain.Persons.Person?>(null));
        var purchaseRepo = Substitute.For<IPurchaseRepository>();
        var subject = new DeletePurchasesByCustomerCommandHandler(personRepo, purchaseRepo);

        // When
        var result = await subject.Handle(new DeletePurchasesByCustomerCommand(1), CancellationToken.None);

        // Then
        Assert.Equal(ResultFailureKind.NotFound, result.FailureKind);
        await purchaseRepo.DidNotReceive().GetActiveByCustomerIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenNoPurchases_ReturnsNotFound()
    {
        // Given
        var personRepo = Substitute.For<IPersonRepository>();
        personRepo.GetByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Domain.Persons.Person?>(DomainBuilders.PersonWithId(1)));
        var purchaseRepo = Substitute.For<IPurchaseRepository>();
        purchaseRepo.GetActiveByCustomerIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<Purchase>>(new List<Purchase>()));
        var subject = new DeletePurchasesByCustomerCommandHandler(personRepo, purchaseRepo);

        // When
        var result = await subject.Handle(new DeletePurchasesByCustomerCommand(1), CancellationToken.None);

        // Then
        Assert.Equal(ResultFailureKind.NotFound, result.FailureKind);
    }

    [Fact]
    public async Task Handle_WhenSecondPurchaseCannotBeDeleted_ReturnsBadRequest()
    {
        // Given — first purchase deletable; second already deleted in domain
        var personRepo = Substitute.For<IPersonRepository>();
        personRepo.GetByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Domain.Persons.Person?>(DomainBuilders.PersonWithId(1)));
        var p1 = DomainBuilders.PurchaseWithId(5, 1, new List<long> { 2 });
        var p2 = DomainBuilders.PurchaseWithId(6, 1, new List<long> { 2 });
        Assert.True(p2.Delete().IsSuccess);
        var purchaseRepo = Substitute.For<IPurchaseRepository>();
        purchaseRepo.GetActiveByCustomerIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<Purchase>>(new List<Purchase> { p1, p2 }));
        purchaseRepo.UpdatePurchaseAsync(Arg.Any<Purchase>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Ok()));
        var subject = new DeletePurchasesByCustomerCommandHandler(personRepo, purchaseRepo);

        // When
        var result = await subject.Handle(new DeletePurchasesByCustomerCommand(1), CancellationToken.None);

        // Then
        Assert.False(result.IsSuccess);
        Assert.Equal(ResultFailureKind.BadRequest, result.FailureKind);
        await purchaseRepo.Received(1).UpdatePurchaseAsync(p1, Arg.Any<CancellationToken>());
        await purchaseRepo.DidNotReceive().UpdatePurchaseAsync(p2, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenUpdateFails_ReturnsThatFailure()
    {
        // Given
        var personRepo = Substitute.For<IPersonRepository>();
        personRepo.GetByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Domain.Persons.Person?>(DomainBuilders.PersonWithId(1)));
        var purchase = DomainBuilders.PurchaseWithId(5, 1, new List<long> { 2 });
        var purchaseRepo = Substitute.For<IPurchaseRepository>();
        purchaseRepo.GetActiveByCustomerIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<Purchase>>(new List<Purchase> { purchase }));
        purchaseRepo.UpdatePurchaseAsync(purchase, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.NotFound("missing")));
        var subject = new DeletePurchasesByCustomerCommandHandler(personRepo, purchaseRepo);

        // When
        var result = await subject.Handle(new DeletePurchasesByCustomerCommand(1), CancellationToken.None);

        // Then
        Assert.False(result.IsSuccess);
        Assert.Equal(ResultFailureKind.NotFound, result.FailureKind);
    }

    [Fact]
    public async Task Handle_WhenValid_ReturnsOk()
    {
        // Given
        var personRepo = Substitute.For<IPersonRepository>();
        personRepo.GetByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Domain.Persons.Person?>(DomainBuilders.PersonWithId(1)));
        var purchase = DomainBuilders.PurchaseWithId(5, 1, new List<long> { 2 });
        var purchaseRepo = Substitute.For<IPurchaseRepository>();
        purchaseRepo.GetActiveByCustomerIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<Purchase>>(new List<Purchase> { purchase }));
        purchaseRepo.UpdatePurchaseAsync(purchase, Arg.Any<CancellationToken>()).Returns(Task.FromResult(Result.Ok()));
        var subject = new DeletePurchasesByCustomerCommandHandler(personRepo, purchaseRepo);

        // When
        var result = await subject.Handle(new DeletePurchasesByCustomerCommand(1), CancellationToken.None);

        // Then
        Assert.True(result.IsSuccess);
        await purchaseRepo.Received(1).UpdatePurchaseAsync(purchase, Arg.Any<CancellationToken>());
    }
}
