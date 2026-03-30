using Bucket.Api.Controllers;
using Bucket.Api.Tests.TestHelpers;
using Bucket.Application.Handlers.Commands.Persons;
using Bucket.Application.Handlers.Queries.Persons;
using Bucket.Common;
using Bucket.Contract;
using Bucket.Contract.Dtos.Persons;
using Bucket.Contract.Requests.Persons;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace Bucket.Api.Tests.Controllers;

public class PersonsControllerTests
{
    [Fact]
    public async Task GetPersons_WhenPaginationInvalid_ReturnsProblem()
    {
        var sender = Substitute.For<ISender>();
        var subject = new PersonsController(sender);
        ControllerTestContext.Attach(subject);

        var action = await subject.GetPersons(new GetPersonsRequest { Page = 0, PageSize = 10 });

        var problem = Assert.IsType<ObjectResult>(action.Result);
        Assert.Equal(400, problem.StatusCode);
    }

    [Fact]
    public async Task GetPersons_WhenValid_ReturnsOk()
    {
        var page = new PagedResponse<PersonDTO>([], 0);
        var sender = Substitute.For<ISender>();
        sender.Send(Arg.Any<GetPersonsQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result<PagedResponse<PersonDTO>>.Success(page)));
        var subject = new PersonsController(sender);
        ControllerTestContext.Attach(subject);

        var action = await subject.GetPersons(new GetPersonsRequest { Page = 1, PageSize = 10 });

        var ok = Assert.IsType<OkObjectResult>(action.Result);
        Assert.Same(page, ok.Value);
    }

    [Fact]
    public async Task GetPerson_WhenNotFound_ReturnsProblem()
    {
        var sender = Substitute.For<ISender>();
        sender.Send(Arg.Any<GetPersonQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result<PersonDTO>.NotFound("missing")));
        var subject = new PersonsController(sender);
        ControllerTestContext.Attach(subject);

        var action = await subject.GetPerson(new GetPersonRequest { Id = 9 });

        var problem = Assert.IsType<ObjectResult>(action.Result);
        Assert.Equal(404, problem.StatusCode);
    }

    [Fact]
    public async Task GetPerson_WhenFound_ReturnsOk()
    {
        var dto = new PersonDTO(1, "A", "B", 1990);
        var sender = Substitute.For<ISender>();
        sender.Send(Arg.Any<GetPersonQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result<PersonDTO>.Success(dto)));
        var subject = new PersonsController(sender);
        ControllerTestContext.Attach(subject);

        var action = await subject.GetPerson(new GetPersonRequest { Id = 1 });

        var ok = Assert.IsType<OkObjectResult>(action.Result);
        Assert.Equal(dto, ok.Value);
    }

    [Fact]
    public async Task AddPerson_WhenFailure_ReturnsProblem()
    {
        var sender = Substitute.For<ISender>();
        sender.Send(Arg.Any<AddPersonCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result<EntityId>.Failure("bad")));
        var subject = new PersonsController(sender);
        ControllerTestContext.Attach(subject);

        var action = await subject.AddPerson(new AddPersonRequest("a", "b", 1990));

        var problem = Assert.IsType<ObjectResult>(action.Result);
        Assert.Equal(400, problem.StatusCode);
    }

    [Fact]
    public async Task AddPerson_WhenSuccess_ReturnsAccepted()
    {
        var sender = Substitute.For<ISender>();
        sender.Send(Arg.Any<AddPersonCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result<EntityId>.Success(new EntityId(42))));
        var subject = new PersonsController(sender);
        ControllerTestContext.Attach(subject);

        var action = await subject.AddPerson(new AddPersonRequest("a", "b", 1990));

        var accepted = Assert.IsType<AcceptedResult>(action.Result);
        Assert.Equal(42L, accepted.Value);
    }

    [Fact]
    public async Task UpdatePerson_WhenFailure_ReturnsProblem()
    {
        var sender = Substitute.For<ISender>();
        sender.Send(Arg.Any<UpdatePersonCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.NotFound("nope")));
        var subject = new PersonsController(sender);
        ControllerTestContext.Attach(subject);

        var action = await subject.UpdatePerson(
            new PersonIdRouteRequest { Id = 1 },
            new UpdatePersonRequest("a", "b", 1990));

        var problem = Assert.IsType<ObjectResult>(action);
        Assert.Equal(404, problem.StatusCode);
    }

    [Fact]
    public async Task UpdatePerson_WhenSuccess_ReturnsAccepted()
    {
        var sender = Substitute.For<ISender>();
        sender.Send(Arg.Any<UpdatePersonCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Ok()));
        var subject = new PersonsController(sender);
        ControllerTestContext.Attach(subject);

        var action = await subject.UpdatePerson(
            new PersonIdRouteRequest { Id = 1 },
            new UpdatePersonRequest("a", "b", 1990));

        Assert.IsType<AcceptedResult>(action);
    }

    [Fact]
    public async Task DeletePerson_WhenFailure_ReturnsProblem()
    {
        var sender = Substitute.For<ISender>();
        sender.Send(Arg.Any<DeletePersonCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.NotFound("nope")));
        var subject = new PersonsController(sender);
        ControllerTestContext.Attach(subject);

        var action = await subject.DeletePerson(new PersonIdRouteRequest { Id = 1 });

        var problem = Assert.IsType<ObjectResult>(action);
        Assert.Equal(404, problem.StatusCode);
    }

    [Fact]
    public async Task DeletePerson_WhenSuccess_ReturnsAccepted()
    {
        var sender = Substitute.For<ISender>();
        sender.Send(Arg.Any<DeletePersonCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Ok()));
        var subject = new PersonsController(sender);
        ControllerTestContext.Attach(subject);

        var action = await subject.DeletePerson(new PersonIdRouteRequest { Id = 1 });

        Assert.IsType<AcceptedResult>(action);
    }
}
