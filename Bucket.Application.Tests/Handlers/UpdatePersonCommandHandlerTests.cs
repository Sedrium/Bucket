using Bucket.Application.Handlers.Commands.Persons;
using Bucket.Application.Repositories;
using Bucket.Application.Tests.TestHelpers;
using Bucket.Common;
using NSubstitute;
using Xunit;

namespace Bucket.Application.Tests.Handlers;

public class UpdatePersonCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenPersonMissing_ReturnsNotFound()
    {
        // Given
        var repo = Substitute.For<IPersonRepository>();
        repo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(Task.FromResult<Domain.Persons.Person?>(null));
        var subject = new UpdatePersonCommandHandler(repo);
        var year = DateOnly.FromDateTime(DateTime.UtcNow).Year - 30;

        // When
        var result = await subject.Handle(new UpdatePersonCommand(1, "X", "Y", year), CancellationToken.None);

        // Then
        Assert.False(result.IsSuccess);
        Assert.Equal(ResultFailureKind.NotFound, result.FailureKind);
        await repo.DidNotReceive().UpdatePersonAsync(Arg.Any<Domain.Persons.Person>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenYearInvalid_ReturnsBadRequest()
    {
        // Given
        var person = DomainBuilders.PersonWithId(1);
        var repo = Substitute.For<IPersonRepository>();
        repo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(Task.FromResult<Domain.Persons.Person?>(person));
        var subject = new UpdatePersonCommandHandler(repo);
        var tooYoungYear = DateOnly.FromDateTime(DateTime.UtcNow).Year - 5;

        // When
        var result = await subject.Handle(new UpdatePersonCommand(1, "X", "Y", tooYoungYear), CancellationToken.None);

        // Then
        Assert.False(result.IsSuccess);
        Assert.Equal(ResultFailureKind.BadRequest, result.FailureKind);
        await repo.DidNotReceive().UpdatePersonAsync(Arg.Any<Domain.Persons.Person>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenDomainUpdateInvalid_ReturnsBadRequest()
    {
        // Given
        var person = DomainBuilders.PersonWithId(1);
        var repo = Substitute.For<IPersonRepository>();
        repo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(Task.FromResult<Domain.Persons.Person?>(person));
        var subject = new UpdatePersonCommandHandler(repo);
        var year = DateOnly.FromDateTime(DateTime.UtcNow).Year - 30;

        // When — empty last name
        var result = await subject.Handle(new UpdatePersonCommand(1, "X", "  ", year), CancellationToken.None);

        // Then
        Assert.False(result.IsSuccess);
        Assert.Equal(ResultFailureKind.BadRequest, result.FailureKind);
        await repo.DidNotReceive().UpdatePersonAsync(Arg.Any<Domain.Persons.Person>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenRepositoryUpdateFails_ReturnsThatFailure()
    {
        // Given
        var person = DomainBuilders.PersonWithId(1);
        var repo = Substitute.For<IPersonRepository>();
        repo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(Task.FromResult<Domain.Persons.Person?>(person));
        repo.UpdatePersonAsync(person, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.NotFound("lost")));
        var subject = new UpdatePersonCommandHandler(repo);
        var year = DateOnly.FromDateTime(DateTime.UtcNow).Year - 30;

        // When
        var result = await subject.Handle(new UpdatePersonCommand(1, "X", "Y", year), CancellationToken.None);

        // Then
        Assert.False(result.IsSuccess);
        Assert.Equal(ResultFailureKind.NotFound, result.FailureKind);
    }

    [Fact]
    public async Task Handle_WhenValid_ReturnsOk()
    {
        // Given
        var person = DomainBuilders.PersonWithId(1);
        var repo = Substitute.For<IPersonRepository>();
        repo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(Task.FromResult<Domain.Persons.Person?>(person));
        repo.UpdatePersonAsync(person, Arg.Any<CancellationToken>()).Returns(Task.FromResult(Result.Ok()));
        var subject = new UpdatePersonCommandHandler(repo);
        var year = DateOnly.FromDateTime(DateTime.UtcNow).Year - 30;

        // When
        var result = await subject.Handle(new UpdatePersonCommand(1, "X", "Y", year), CancellationToken.None);

        // Then
        Assert.True(result.IsSuccess);
        await repo.Received(1).UpdatePersonAsync(person, Arg.Any<CancellationToken>());
    }
}
