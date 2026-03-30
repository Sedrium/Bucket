using Bucket.Domain.Persons;
using Bucket.Domain.Products;
using Bucket.Domain.Purchases;
using Xunit;

namespace Bucket.Application.Tests.TestHelpers;

internal static class DomainBuilders
{
    internal static YearOfBirth ValidYear(int yearsAgo = 30)
    {
        var y = YearOfBirth.Create(DateOnly.FromDateTime(DateTime.UtcNow).Year - yearsAgo);
        Assert.True(y.IsSuccess);
        return y.Value!;
    }

    internal static Person PersonWithId(long id)
    {
        var p = Person.Create("Ann", "Smith", ValidYear()).Value!;
        p.SetId(id);
        return p;
    }

    internal static Product ProductWithId(long id, string name = "Item", string type = "T", double price = 5)
    {
        var p = Product.Create(name, type, price).Value!;
        p.SetId(id);
        return p;
    }

    internal static Purchase PurchaseWithId(long id, long customerId, IReadOnlyList<long> productIds)
    {
        var pur = Purchase.Create(customerId, productIds).Value!;
        pur.SetId(id);
        return pur;
    }
}
