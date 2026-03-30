using Bucket.Domain.Products;
using Xunit;

namespace Bucket.Domain.Tests;

public class ProductTests
{
    [Fact]
    public void Create_WithValidData_ReturnsSuccess()
    {
        var result = Product.Create("Hammer", "Tool", 9.99);

        Assert.True(result.IsSuccess);
        Assert.Equal("Hammer", result.Value!.Name);
    }

    [Fact]
    public void Create_WithNegativePrice_ReturnsFailure()
    {
        var result = Product.Create("X", "Y", -1);

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void Update_WithoutIdentity_ReturnsFailure()
    {
        var product = Product.Create("A", "B", 1).Value!;

        var result = product.Update("C", "D", 2);

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void Delete_WithIdentity_ReturnsSuccess()
    {
        var product = Product.Create("A", "B", 1).Value!;
        product.SetId(1);

        var result = product.Delete();

        Assert.True(result.IsSuccess);
        Assert.True(product.IsDeleted);
    }
}
