using Bucket.Api.Controllers;
using Bucket.Api.Tests.TestHelpers;
using Bucket.Application.Handlers.Commands.Products;
using Bucket.Application.Handlers.Queries.Products;
using Bucket.Common;
using Bucket.Contract;
using Bucket.Contract.Dtos.Products;
using Bucket.Contract.Requests.Products;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace Bucket.Api.Tests.Controllers;

public class ProductsControllerTests
{
    [Fact]
    public async Task GetProducts_WhenPaginationInvalid_ReturnsProblem()
    {
        var sender = Substitute.For<ISender>();
        var subject = new ProductsController(sender);
        ControllerTestContext.Attach(subject);

        var action = await subject.GetProducts(new GetProductsRequest { Page = 1, PageSize = 0 });

        var problem = Assert.IsType<ObjectResult>(action.Result);
        Assert.Equal(400, problem.StatusCode);
    }

    [Fact]
    public async Task GetProducts_WhenValid_ReturnsOk()
    {
        var page = new PagedResponse<ProductDTO>([], 0);
        var sender = Substitute.For<ISender>();
        sender.Send(Arg.Any<GetProductsQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result<PagedResponse<ProductDTO>>.Success(page)));
        var subject = new ProductsController(sender);
        ControllerTestContext.Attach(subject);

        var action = await subject.GetProducts(new GetProductsRequest { Page = 1, PageSize = 10 });

        var ok = Assert.IsType<OkObjectResult>(action.Result);
        Assert.Same(page, ok.Value);
    }

    [Fact]
    public async Task GetProduct_WhenNotFound_ReturnsProblem()
    {
        var sender = Substitute.For<ISender>();
        sender.Send(Arg.Any<GetProductQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result<ProductDTO>.NotFound("missing")));
        var subject = new ProductsController(sender);
        ControllerTestContext.Attach(subject);

        var action = await subject.GetProduct(new GetProductRequest { Id = 9 });

        var problem = Assert.IsType<ObjectResult>(action.Result);
        Assert.Equal(404, problem.StatusCode);
    }

    [Fact]
    public async Task GetProduct_WhenFound_ReturnsOk()
    {
        var dto = new ProductDTO(1, "n", "t", 9.99);
        var sender = Substitute.For<ISender>();
        sender.Send(Arg.Any<GetProductQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result<ProductDTO>.Success(dto)));
        var subject = new ProductsController(sender);
        ControllerTestContext.Attach(subject);

        var action = await subject.GetProduct(new GetProductRequest { Id = 1 });

        var ok = Assert.IsType<OkObjectResult>(action.Result);
        Assert.Equal(dto, ok.Value);
    }

    [Fact]
    public async Task AddProduct_WhenFailure_ReturnsProblem()
    {
        var sender = Substitute.For<ISender>();
        sender.Send(Arg.Any<AddProductCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result<EntityId>.Failure("bad")));
        var subject = new ProductsController(sender);
        ControllerTestContext.Attach(subject);

        var action = await subject.AddProduct(new AddProductRequest("n", "t", 1));

        var problem = Assert.IsType<ObjectResult>(action.Result);
        Assert.Equal(400, problem.StatusCode);
    }

    [Fact]
    public async Task AddProduct_WhenSuccess_ReturnsAccepted()
    {
        var sender = Substitute.For<ISender>();
        sender.Send(Arg.Any<AddProductCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result<EntityId>.Success(new EntityId(7))));
        var subject = new ProductsController(sender);
        ControllerTestContext.Attach(subject);

        var action = await subject.AddProduct(new AddProductRequest("n", "t", 1));

        var accepted = Assert.IsType<AcceptedResult>(action.Result);
        Assert.Equal(7L, accepted.Value);
    }

    [Fact]
    public async Task UpdateProduct_WhenFailure_ReturnsProblem()
    {
        var sender = Substitute.For<ISender>();
        sender.Send(Arg.Any<UpdateProductCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.NotFound("nope")));
        var subject = new ProductsController(sender);
        ControllerTestContext.Attach(subject);

        var action = await subject.UpdateProduct(
            new ProductIdRouteRequest { Id = 1 },
            new UpdateProductRequest("n", "t", 1));

        var problem = Assert.IsType<ObjectResult>(action);
        Assert.Equal(404, problem.StatusCode);
    }

    [Fact]
    public async Task UpdateProduct_WhenSuccess_ReturnsAccepted()
    {
        var sender = Substitute.For<ISender>();
        sender.Send(Arg.Any<UpdateProductCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Ok()));
        var subject = new ProductsController(sender);
        ControllerTestContext.Attach(subject);

        var action = await subject.UpdateProduct(
            new ProductIdRouteRequest { Id = 1 },
            new UpdateProductRequest("n", "t", 1));

        Assert.IsType<AcceptedResult>(action);
    }

    [Fact]
    public async Task DeleteProduct_WhenFailure_ReturnsProblem()
    {
        var sender = Substitute.For<ISender>();
        sender.Send(Arg.Any<DeleteProductCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.NotFound("nope")));
        var subject = new ProductsController(sender);
        ControllerTestContext.Attach(subject);

        var action = await subject.DeleteProduct(new ProductIdRouteRequest { Id = 1 });

        var problem = Assert.IsType<ObjectResult>(action);
        Assert.Equal(404, problem.StatusCode);
    }

    [Fact]
    public async Task DeleteProduct_WhenSuccess_ReturnsAccepted()
    {
        var sender = Substitute.For<ISender>();
        sender.Send(Arg.Any<DeleteProductCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Ok()));
        var subject = new ProductsController(sender);
        ControllerTestContext.Attach(subject);

        var action = await subject.DeleteProduct(new ProductIdRouteRequest { Id = 1 });

        Assert.IsType<AcceptedResult>(action);
    }
}
