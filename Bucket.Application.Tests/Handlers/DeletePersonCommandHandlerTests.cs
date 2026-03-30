using Bucket.Application.Handlers.Commands.Persons;
using Bucket.Application.Repositories;
using Bucket.Application.Tests.TestHelpers;
using Bucket.Common;
using Bucket.Domain.Persons;
using NSubstitute;
using Xunit;

namespace Bucket.Application.Tests.Handlers;

public class DeletePersonCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenPersonMissing_ReturnsNotFound()
    {
        // Given
        var repo = Substitute.For<IPersonRepository>();
        repo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(Task.FromResult<Person?>(null));
        var subject = new DeletePersonCommandHandler(repo);

        // When
        var result = await subject.Handle(new DeletePersonCommand(1), CancellationToken.None);

        // Then
        Assert.Equal(ResultFailureKind.NotFound, result.FailureKind);
        await repo.DidNotReceive().UpdatePersonAsync(Arg.Any<Person>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenPersonHasNoIdentity_ReturnsBadRequest()
    {
        // Given — Person without SetId: Delete() fails inside domain
        var personWithoutId = Person.Create("A", "B", DomainBuilders.ValidYear()).Value!;
        var repo = Substitute.For<IPersonRepository>();
        repo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(Task.FromResult<Person?>(personWithoutId));
        var subject = new DeletePersonCommandHandler(repo);

        // When
        var result = await subject.Handle(new DeletePersonCommand(1), CancellationToken.None);

        // Then
        Assert.False(result.IsSuccess);
        Assert.Equal(ResultFailureKind.BadRequest, result.FailureKind);
        await repo.DidNotReceive().UpdatePersonAsync(Arg.Any<Person>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenAlreadyDeleted_ReturnsBadRequest()
    {
        // Given
        var person = DomainBuilders.PersonWithId(1);
        Assert.True(person.Delete().IsSuccess);
        var repo = Substitute.For<IPersonRepository>();
        repo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(Task.FromResult<Person?>(person));
        var subject = new DeletePersonCommandHandler(repo);

        // When
        var result = await subject.Handle(new DeletePersonCommand(1), CancellationToken.None);

        // Then
        Assert.False(result.IsSuccess);
        Assert.Equal(ResultFailureKind.BadRequest, result.FailureKind);
        await repo.DidNotReceive().UpdatePersonAsync(Arg.Any<Person>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenRepositoryUpdateFails_ReturnsThatFailure()
    {
        // Given
        var person = DomainBuilders.PersonWithId(1);
        var repo = Substitute.For<IPersonRepository>();
        repo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(Task.FromResult<Person?>(person));
        repo.UpdatePersonAsync(person, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Fail("concurrency")));
        var subject = new DeletePersonCommandHandler(repo);

        // When
        var result = await subject.Handle(new DeletePersonCommand(1), CancellationToken.None);

        // Then
        Assert.False(result.IsSuccess);
        Assert.Equal(ResultFailureKind.BadRequest, result.FailureKind);
    }

    [Fact]
    public async Task Handle_WhenValid_ReturnsOk()
    {
        // Given
        var person = DomainBuilders.PersonWithId(1);
        var repo = Substitute.For<IPersonRepository>();
        repo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(Task.FromResult<Person?>(person));
        repo.UpdatePersonAsync(person, Arg.Any<CancellationToken>()).Returns(Task.FromResult(Result.Ok()));
        var subject = new DeletePersonCommandHandler(repo);

        // When
        var result = await subject.Handle(new DeletePersonCommand(1), CancellationToken.None);

        // Then
        Assert.True(result.IsSuccess);
        await repo.Received(1).UpdatePersonAsync(person, Arg.Any<CancellationToken>());
    }
}
