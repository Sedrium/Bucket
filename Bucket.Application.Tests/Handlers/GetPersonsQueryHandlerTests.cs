using Bucket.Application.Handlers.Queries.Persons;
using Bucket.Application.Queries;
using Bucket.Common;
using Bucket.Contract;
using Bucket.Contract.Dtos.Persons;
using NSubstitute;
using Xunit;

namespace Bucket.Application.Tests.Handlers;

public class GetPersonsQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenQueryReturnsSuccess_ReturnsSameResult_AndDelegates()
    {
        // Given
        var pagination = Pagination.Create(1, 10).Value!;
        var expected = Result<PagedResponse<PersonDTO>>.Success(
            new PagedResponse<PersonDTO>(Array.Empty<PersonDTO>(), 0));
        var query = Substitute.For<IPersonQuery>();
        query.GetPersonsAsync(pagination, Arg.Any<CancellationToken>()).Returns(Task.FromResult(expected));
        var subject = new GetPersonsQueryHandler(query);

        // When
        var result = await subject.Handle(new GetPersonsQuery(pagination), CancellationToken.None);

        // Then
        Assert.True(result.IsSuccess);
        Assert.Same(expected, result);
        await query.Received(1).GetPersonsAsync(pagination, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenQueryReturnsFailure_ReturnsThatFailure_AndDelegates()
    {
        // Given
        var pagination = Pagination.Create(2, 5).Value!;
        var expected = Result<PagedResponse<PersonDTO>>.Failure("query failed");
        var query = Substitute.For<IPersonQuery>();
        query.GetPersonsAsync(pagination, Arg.Any<CancellationToken>()).Returns(Task.FromResult(expected));
        var subject = new GetPersonsQueryHandler(query);

        // When
        var result = await subject.Handle(new GetPersonsQuery(pagination), CancellationToken.None);

        // Then
        Assert.False(result.IsSuccess);
        Assert.Equal(ResultFailureKind.BadRequest, result.FailureKind);
        Assert.Same(expected, result);
        await query.Received(1).GetPersonsAsync(pagination, Arg.Any<CancellationToken>());
    }
}
