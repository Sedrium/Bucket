using Bucket.Domain.Persons;
using Bucket.Domain.Products;
using Bucket.Domain.Purchases;

namespace Bucket.Infrastructure.Data;

public sealed class Data
{
    public static Data Instance { get; } = new();

    private Data()
    {
    }

    public List<Product> Products { get; } =
    [
        CreateProduct(1, "Basic license", "Software", 99.99),
        CreateProduct(2, "Pro license", "Software", 299.99),
        CreateProduct(3, "Support pack", "Service", 49.50),
        CreateProduct(4, "USB sensor", "Hardware", 24.00),
        CreateProduct(5, "API bundle", "Software", 199.00),
        CreateProduct(6, "Training day", "Service", 800.00)
    ];

    private static Product CreateProduct(long id, string name, string type, double price)
    {
        var result = Product.Create(name, type, price);
        if (!result.IsSuccess)
        {
            throw new InvalidOperationException($"Seed product invalid: {result.Error}");
        }

        result.Value!.SetId(id);
        return result.Value!;
    }

    public List<Person> Persons { get; } =
    [
        Create(1, "John", "Doe", new DateOnly(1980, 6, 15)),
        Create(2, "Jane", "Doe", new DateOnly(1985, 3, 22)),
        Create(3, "Bob", "Smith", new DateOnly(1990, 11, 8)),
        Create(4, "Alice", "Johnson", new DateOnly(1995, 1, 30)),
        Create(5, "Mike", "Brown", new DateOnly(1982, 9, 5)),
        Create(6, "Samantha", "Davis", new DateOnly(1987, 7, 19)),
        Create(7, "David", "Wilson", new DateOnly(1992, 4, 12)),
        Create(8, "Emily", "Taylor", new DateOnly(1997, 12, 1)),
        Create(9, "Chris", "Anderson", new DateOnly(1984, 2, 28)),
        Create(10, "Jessica", "Thomas", new DateOnly(1989, 8, 14))
    ];

    private static Person Create(long id, string firstName, string lastName, DateOnly dateOfBirth)
    {
        var yearResult = YearOfBirth.Create(dateOfBirth.Year);
        if (!yearResult.IsSuccess)
        {
            throw new InvalidOperationException($"Seed year invalid: {yearResult.Error}");
        }

        var result = Person.Create(firstName, lastName, yearResult.Value!);

        if (!result.IsSuccess)
        {
            throw new InvalidOperationException($"Seed person invalid: {result.Error}");
        }

        result.Value!.SetId(id);

        return result.Value!;
    }

    public List<Purchase> Purchases { get; } =
    [
        CreatePurchase(1, 1, [1L, 2L]),
        CreatePurchase(2, 2, [3L, 4L]),
        CreatePurchase(3, 1, [5L])
    ];

    private static Purchase CreatePurchase(long id, long customerId, long[] productIds)
    {
        var result = Purchase.Create(customerId, productIds);
        if (!result.IsSuccess)
        {
            throw new InvalidOperationException($"Seed purchase invalid: {result.Error}");
        }

        result.Value!.SetId(id);
        return result.Value!;
    }
}
