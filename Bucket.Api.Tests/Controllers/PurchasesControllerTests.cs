using Bucket.Api.Controllers;
using Bucket.Api.Tests.TestHelpers;
using Bucket.Application.Handlers.Commands.Purchases;
using Bucket.Application.Handlers.Queries.Purchases;
using Bucket.Common;
using Bucket.Contract;
using Bucket.Contract.Dtos.Purchases;
using Bucket.Contract.Requests.Purchases;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace Bucket.Api.Tests.Controllers;

public class PurchasesControllerTests
{
    [Fact]
    public async Task GetPurchases_WhenPaginationInvalid_ReturnsProblem()
    {
        var sender = Substitute.For<ISender>();
        var subject = new PurchasesController(sender);
        ControllerTestContext.Attach(subject);

        var action = await subject.GetPurchases(new GetPurchasesRequest { Page = 1, PageSize = 200 });

        var problem = Assert.IsType<ObjectResult>(action.Result);
        Assert.Equal(400, problem.StatusCode);
    }

    [Fact]
    public async Task GetPurchases_WhenValid_ReturnsOk()
    {
        var page = new PagedResponse<PurchaseDTO>([], 0);
        var sender = Substitute.For<ISender>();
        sender.Send(Arg.Any<GetPurchasesQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result<PagedResponse<PurchaseDTO>>.Success(page)));
        var subject = new PurchasesController(sender);
        ControllerTestContext.Attach(subject);

        var action = await subject.GetPurchases(new GetPurchasesRequest { Page = 1, PageSize = 10 });

        var ok = Assert.IsType<OkObjectResult>(action.Result);
        Assert.Same(page, ok.Value);
    }

    [Fact]
    public async Task GetPurchase_WhenNotFound_ReturnsProblem()
    {
        var sender = Substitute.For<ISender>();
        sender.Send(Arg.Any<GetPurchaseQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result<PurchaseDTO>.NotFound("missing")));
        var subject = new PurchasesController(sender);
        ControllerTestContext.Attach(subject);

        var action = await subject.GetPurchase(new GetPurchaseRequest { Id = 9 });

        var problem = Assert.IsType<ObjectResult>(action.Result);
        Assert.Equal(404, problem.StatusCode);
    }

    [Fact]
    public async Task GetPurchase_WhenFound_ReturnsOk()
    {
        var dto = new PurchaseDTO(1, 2, [3L, 4L]);
        var sender = Substitute.For<ISender>();
        sender.Send(Arg.Any<GetPurchaseQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result<PurchaseDTO>.Success(dto)));
        var subject = new PurchasesController(sender);
        ControllerTestContext.Attach(subject);

        var action = await subject.GetPurchase(new GetPurchaseRequest { Id = 1 });

        var ok = Assert.IsType<OkObjectResult>(action.Result);
        Assert.Equal(dto, ok.Value);
    }

    [Fact]
    public async Task AddPurchase_WhenFailure_ReturnsProblem()
    {
        var sender = Substitute.For<ISender>();
        sender.Send(Arg.Any<AddPurchaseCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result<EntityId>.NotFound("customer")));
        var subject = new PurchasesController(sender);
        ControllerTestContext.Attach(subject);

        var action = await subject.AddPurchase(new AddPurchaseRequest(1, [2L]));

        var problem = Assert.IsType<ObjectResult>(action.Result);
        Assert.Equal(404, problem.StatusCode);
    }

    [Fact]
    public async Task AddPurchase_WhenSuccess_ReturnsAccepted()
    {
        var sender = Substitute.For<ISender>();
        sender.Send(Arg.Any<AddPurchaseCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result<EntityId>.Success(new EntityId(99))));
        var subject = new PurchasesController(sender);
        ControllerTestContext.Attach(subject);

        var action = await subject.AddPurchase(new AddPurchaseRequest(1, [2L]));

        var accepted = Assert.IsType<AcceptedResult>(action.Result);
        Assert.Equal(99L, accepted.Value);
    }

    [Fact]
    public async Task DeletePurchasesByCustomer_WhenFailure_ReturnsProblem()
    {
        var sender = Substitute.For<ISender>();
        sender.Send(Arg.Any<DeletePurchasesByCustomerCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.NotFound("none")));
        var subject = new PurchasesController(sender);
        ControllerTestContext.Attach(subject);

        var action = await subject.DeletePurchasesByCustomer(new CustomerIdRouteRequest { CustomerId = 1 });

        var problem = Assert.IsType<ObjectResult>(action);
        Assert.Equal(404, problem.StatusCode);
    }

    [Fact]
    public async Task DeletePurchasesByCustomer_WhenSuccess_ReturnsAccepted()
    {
        var sender = Substitute.For<ISender>();
        sender.Send(Arg.Any<DeletePurchasesByCustomerCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Ok()));
        var subject = new PurchasesController(sender);
        ControllerTestContext.Attach(subject);

        var action = await subject.DeletePurchasesByCustomer(new CustomerIdRouteRequest { CustomerId = 1 });

        Assert.IsType<AcceptedResult>(action);
    }

    [Fact]
    public async Task DeletePurchase_WhenFailure_ReturnsProblem()
    {
        var sender = Substitute.For<ISender>();
        sender.Send(Arg.Any<DeletePurchaseCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.NotFound("nope")));
        var subject = new PurchasesController(sender);
        ControllerTestContext.Attach(subject);

        var action = await subject.DeletePurchase(new PurchaseIdRouteRequest { Id = 1 });

        var problem = Assert.IsType<ObjectResult>(action);
        Assert.Equal(404, problem.StatusCode);
    }

    [Fact]
    public async Task DeletePurchase_WhenSuccess_ReturnsAccepted()
    {
        var sender = Substitute.For<ISender>();
        sender.Send(Arg.Any<DeletePurchaseCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Ok()));
        var subject = new PurchasesController(sender);
        ControllerTestContext.Attach(subject);

        var action = await subject.DeletePurchase(new PurchaseIdRouteRequest { Id = 1 });

        Assert.IsType<AcceptedResult>(action);
    }
}
