using Bucket.Common;
using Bucket.Domain.Persons;
using Xunit;

namespace Bucket.Domain.Tests;

public class PersonTests
{
    private static YearOfBirth ValidYearOfBirth()
    {
        var y = YearOfBirth.Create(DateOnly.FromDateTime(DateTime.UtcNow).Year - 30);
        Assert.True(y.IsSuccess);
        return y.Value!;
    }

    [Fact]
    public void Create_WithValidData_ReturnsSuccess()
    {
        var result = Person.Create("Jane", "Doe", ValidYearOfBirth());

        Assert.True(result.IsSuccess);
        Assert.Equal("Jane", result.Value!.FirstName);
        Assert.Equal("Doe", result.Value.LastName);
    }

    [Fact]
    public void Create_WithEmptyFirstName_ReturnsFailure()
    {
        var result = Person.Create("", "Doe", ValidYearOfBirth());

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void Update_WithoutIdentity_ReturnsFailure()
    {
        var person = Person.Create("A", "B", ValidYearOfBirth()).Value!;

        var result = person.Update("C", "D", ValidYearOfBirth());

        Assert.False(result.IsSuccess);
        Assert.Equal("Person has no identity.", result.Error);
    }

    [Fact]
    public void Update_WithIdentity_ReturnsSuccess()
    {
        var person = Person.Create("A", "B", ValidYearOfBirth()).Value!;
        person.SetId(1);

        var result = person.Update("C", "D", ValidYearOfBirth());

        Assert.True(result.IsSuccess);
        Assert.Equal("C", person.FirstName);
    }

    [Fact]
    public void Delete_WhenAlreadyDeleted_ReturnsFailure()
    {
        var person = Person.Create("A", "B", ValidYearOfBirth()).Value!;
        person.SetId(1);
        Assert.True(person.Delete().IsSuccess);

        var result = person.Delete();

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void SetId_SetsIdOnce()
    {
        var person = Person.Create("A", "B", ValidYearOfBirth()).Value!;
        person.SetId(10);
        person.SetId(99);

        Assert.Equal(10L, person.Id);
    }
}
