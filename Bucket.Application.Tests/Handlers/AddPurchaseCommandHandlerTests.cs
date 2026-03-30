using Bucket.Application.Handlers.Commands.Purchases;
using Bucket.Application.Repositories;
using Bucket.Application.Tests.TestHelpers;
using Bucket.Common;
using Bucket.Domain.Purchases;
using NSubstitute;
using Xunit;

namespace Bucket.Application.Tests.Handlers;

public class AddPurchaseCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenCustomerMissing_ReturnsNotFound()
    {
        // Given
        var personRepo = Substitute.For<IPersonRepository>();
        personRepo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(Task.FromResult<Domain.Persons.Person?>(null));
        var productRepo = Substitute.For<IProductRepository>();
        var purchaseRepo = Substitute.For<IPurchaseRepository>();
        var subject = new AddPurchaseCommandHandler(personRepo, productRepo, purchaseRepo);

        // When
        var result = await subject.Handle(new AddPurchaseCommand(1, new List<long> { 2 }), CancellationToken.None);

        // Then
        Assert.Equal(ResultFailureKind.NotFound, result.FailureKind);
        await purchaseRepo.DidNotReceive().AddPurchaseAsync(Arg.Any<Purchase>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenProductCountMismatch_ReturnsNotFound()
    {
        // Given
        var personRepo = Substitute.For<IPersonRepository>();
        personRepo.GetByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Domain.Persons.Person?>(DomainBuilders.PersonWithId(1)));
        var productRepo = Substitute.For<IProductRepository>();
        productRepo.GetByIdsAsync(Arg.Any<IReadOnlyCollection<long>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<Domain.Products.Product>>(new List<Domain.Products.Product>()));
        var purchaseRepo = Substitute.For<IPurchaseRepository>();
        var subject = new AddPurchaseCommandHandler(personRepo, productRepo, purchaseRepo);

        // When
        var result = await subject.Handle(new AddPurchaseCommand(1, new List<long> { 2, 3 }), CancellationToken.None);

        // Then
        Assert.Equal(ResultFailureKind.NotFound, result.FailureKind);
        await purchaseRepo.DidNotReceive().AddPurchaseAsync(Arg.Any<Purchase>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenProductIdsEmpty_ReturnsFailure_FromPurchaseCreate()
    {
        // Given — distinct empty, repo returns no products, counts match 0==0, Purchase.Create fails on empty lines
        var personRepo = Substitute.For<IPersonRepository>();
        personRepo.GetByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Domain.Persons.Person?>(DomainBuilders.PersonWithId(1)));
        var productRepo = Substitute.For<IProductRepository>();
        productRepo.GetByIdsAsync(Arg.Any<IReadOnlyCollection<long>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<Domain.Products.Product>>(new List<Domain.Products.Product>()));
        var purchaseRepo = Substitute.For<IPurchaseRepository>();
        var subject = new AddPurchaseCommandHandler(personRepo, productRepo, purchaseRepo);

        // When
        var result = await subject.Handle(new AddPurchaseCommand(1, Array.Empty<long>()), CancellationToken.None);

        // Then
        Assert.False(result.IsSuccess);
        Assert.Equal(ResultFailureKind.BadRequest, result.FailureKind);
        await purchaseRepo.DidNotReceive().AddPurchaseAsync(Arg.Any<Purchase>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenTooManyProductLines_ReturnsFailure_FromPurchaseCreate()
    {
        // Given
        var ids = Enumerable.Range(1, 101).Select(i => (long)i).ToList();
        var products = ids.Select(id => DomainBuilders.ProductWithId(id)).ToList();
        var personRepo = Substitute.For<IPersonRepository>();
        personRepo.GetByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Domain.Persons.Person?>(DomainBuilders.PersonWithId(1)));
        var productRepo = Substitute.For<IProductRepository>();
        productRepo.GetByIdsAsync(Arg.Any<IReadOnlyCollection<long>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<Domain.Products.Product>>(products));
        var purchaseRepo = Substitute.For<IPurchaseRepository>();
        var subject = new AddPurchaseCommandHandler(personRepo, productRepo, purchaseRepo);

        // When
        var result = await subject.Handle(new AddPurchaseCommand(1, ids), CancellationToken.None);

        // Then
        Assert.False(result.IsSuccess);
        Assert.Equal(ResultFailureKind.BadRequest, result.FailureKind);
        await purchaseRepo.DidNotReceive().AddPurchaseAsync(Arg.Any<Purchase>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenValid_ReturnsEntityId()
    {
        // Given
        var personRepo = Substitute.For<IPersonRepository>();
        personRepo.GetByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Domain.Persons.Person?>(DomainBuilders.PersonWithId(1)));
        var p = DomainBuilders.ProductWithId(2);
        var productRepo = Substitute.For<IProductRepository>();
        productRepo.GetByIdsAsync(Arg.Any<IReadOnlyCollection<long>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<Domain.Products.Product>>(new List<Domain.Products.Product> { p }));
        var purchaseRepo = Substitute.For<IPurchaseRepository>();
        purchaseRepo.AddPurchaseAsync(Arg.Any<Purchase>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result<long>.Success(77L)));
        var subject = new AddPurchaseCommandHandler(personRepo, productRepo, purchaseRepo);

        // When
        var result = await subject.Handle(new AddPurchaseCommand(1, new List<long> { 2 }), CancellationToken.None);

        // Then
        Assert.True(result.IsSuccess);
        Assert.Equal(77L, result.Value!.Value);
        await purchaseRepo.Received(1).AddPurchaseAsync(Arg.Any<Purchase>(), Arg.Any<CancellationToken>());
    }
}
