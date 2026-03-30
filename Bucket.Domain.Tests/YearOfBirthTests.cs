using Bucket.Common;
using Bucket.Domain.Persons;
using Xunit;

namespace Bucket.Domain.Tests;

public class YearOfBirthTests
{
    [Fact]
    public void Create_WithValidYear_ReturnsSuccess()
    {
        var year = DateOnly.FromDateTime(DateTime.UtcNow).Year - 30;

        var result = YearOfBirth.Create(year);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }

    [Fact]
    public void Create_WithTooRecentYear_ReturnsFailure()
    {
        var year = DateOnly.FromDateTime(DateTime.UtcNow).Year - 5;

        var result = YearOfBirth.Create(year);

        Assert.False(result.IsSuccess);
        Assert.Contains("Age must be greater than", result.Error, StringComparison.Ordinal);
    }

    [Fact]
    public void Create_WithInvalidCalendarYear_ReturnsFailure()
    {
        var result = YearOfBirth.Create(1800);

        Assert.False(result.IsSuccess);
    }
}
