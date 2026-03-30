using Bucket.Application.Handlers.Commands.Persons;
using Bucket.Application.Repositories;
using Bucket.Application.Tests.TestHelpers;
using Bucket.Common;
using NSubstitute;
using Xunit;

namespace Bucket.Application.Tests.Handlers;

public class AddPersonCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenYearInvalid_ReturnsFailure_AndDoesNotCallRepository()
    {
        // Given
        var repo = Substitute.For<IPersonRepository>();
        var subject = new AddPersonCommandHandler(repo);
        var tooYoungYear = DateOnly.FromDateTime(DateTime.UtcNow).Year - 5;

        // When
        var result = await subject.Handle(new AddPersonCommand("A", "B", tooYoungYear), CancellationToken.None);

        // Then
        Assert.False(result.IsSuccess);
        Assert.Equal(ResultFailureKind.BadRequest, result.FailureKind);
        await repo.DidNotReceive().AddPersonAsync(Arg.Any<Domain.Persons.Person>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenPersonValidationFails_ReturnsFailure_AndDoesNotCallRepository()
    {
        // Given
        var repo = Substitute.For<IPersonRepository>();
        var subject = new AddPersonCommandHandler(repo);
        var year = DateOnly.FromDateTime(DateTime.UtcNow).Year - 30;

        // When — empty first name after trim
        var result = await subject.Handle(new AddPersonCommand("  ", "B", year), CancellationToken.None);

        // Then
        Assert.False(result.IsSuccess);
        Assert.Equal(ResultFailureKind.BadRequest, result.FailureKind);
        await repo.DidNotReceive().AddPersonAsync(Arg.Any<Domain.Persons.Person>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenRepositoryReturnsNotFound_ReturnsNotFound()
    {
        // Given
        var year = DateOnly.FromDateTime(DateTime.UtcNow).Year - 30;
        var repo = Substitute.For<IPersonRepository>();
        repo.AddPersonAsync(Arg.Any<Domain.Persons.Person>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result<long>.NotFound("conflict")));
        var subject = new AddPersonCommandHandler(repo);

        // When
        var result = await subject.Handle(new AddPersonCommand("A", "B", year), CancellationToken.None);

        // Then
        Assert.False(result.IsSuccess);
        Assert.Equal(ResultFailureKind.NotFound, result.FailureKind);
    }

    [Fact]
    public async Task Handle_WhenRepositoryReturnsFailure_ReturnsBadRequest()
    {
        // Given
        var year = DateOnly.FromDateTime(DateTime.UtcNow).Year - 30;
        var repo = Substitute.For<IPersonRepository>();
        repo.AddPersonAsync(Arg.Any<Domain.Persons.Person>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result<long>.Failure("db error")));
        var subject = new AddPersonCommandHandler(repo);

        // When
        var result = await subject.Handle(new AddPersonCommand("A", "B", year), CancellationToken.None);

        // Then
        Assert.False(result.IsSuccess);
        Assert.Equal(ResultFailureKind.BadRequest, result.FailureKind);
    }

    [Fact]
    public async Task Handle_WhenValid_ReturnsEntityId()
    {
        // Given
        var year = DateOnly.FromDateTime(DateTime.UtcNow).Year - 30;
        var repo = Substitute.For<IPersonRepository>();
        repo.AddPersonAsync(Arg.Any<Domain.Persons.Person>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result<long>.Success(42L)));
        var subject = new AddPersonCommandHandler(repo);

        // When
        var result = await subject.Handle(new AddPersonCommand("A", "B", year), CancellationToken.None);

        // Then
        Assert.True(result.IsSuccess);
        Assert.Equal(42L, result.Value!.Value);
    }
}
