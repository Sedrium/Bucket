using Bucket.Domain.Purchases;
using Xunit;

namespace Bucket.Domain.Tests;

public class PurchaseTests
{
    [Fact]
    public void Create_WithValidData_ReturnsSuccess()
    {
        var result = Purchase.Create(1, new List<long> { 10, 20 });

        Assert.True(result.IsSuccess);
        Assert.Equal(1L, result.Value!.CustomerId);
        Assert.Equal(2, result.Value.ProductIds.Count);
    }

    [Fact]
    public void Create_WithEmptyProductIds_ReturnsFailure()
    {
        var result = Purchase.Create(1, new List<long>());

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void Create_WithInvalidCustomerId_ReturnsFailure()
    {
        var result = Purchase.Create(0, new List<long> { 1 });

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void Delete_WhenAlreadyDeleted_ReturnsFailure()
    {
        var purchase = Purchase.Create(1, new List<long> { 1 }).Value!;
        purchase.SetId(1);
        Assert.True(purchase.Delete().IsSuccess);

        var result = purchase.Delete();

        Assert.False(result.IsSuccess);
    }
}
