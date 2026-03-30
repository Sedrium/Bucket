using Bucket.Application.Handlers.Queries.Persons;
using Bucket.Application.Queries;
using Bucket.Common;
using Bucket.Contract.Dtos.Persons;
using NSubstitute;
using Xunit;

namespace Bucket.Application.Tests.Handlers;

public class GetPersonQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenFound_ReturnsSuccess()
    {
        // Given
        var query = Substitute.For<IPersonQuery>();
        var dto = new PersonDTO(1, "A", "B", 1990);
        query.GetPersonByIdAsync(1, Arg.Any<CancellationToken>()).Returns(Task.FromResult<PersonDTO?>(dto));
        var subject = new GetPersonQueryHandler(query);

        // When
        var result = await subject.Handle(new GetPersonQuery(1), CancellationToken.None);

        // Then
        Assert.True(result.IsSuccess);
        Assert.Equal(1L, result.Value!.Id);
        Assert.Equal("A", result.Value.Firstname);
    }

    [Fact]
    public async Task Handle_WhenMissing_ReturnsNotFound()
    {
        // Given
        var query = Substitute.For<IPersonQuery>();
        query.GetPersonByIdAsync(1, Arg.Any<CancellationToken>()).Returns(Task.FromResult<PersonDTO?>(null));
        var subject = new GetPersonQueryHandler(query);

        // When
        var result = await subject.Handle(new GetPersonQuery(1), CancellationToken.None);

        // Then
        Assert.False(result.IsSuccess);
        Assert.Equal(ResultFailureKind.NotFound, result.FailureKind);
    }
}
