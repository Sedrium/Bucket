using Bucket.Common;

namespace Bucket.Domain.Products;

public class Product
{
    private const int MaxNameLength = 200;
    private const int MaxTypeLength = 100;

    public long? Id { get; private set; }
    public string Name { get; private set; }
    public string Type { get; private set; }
    public double Price { get; private set; }
    public bool IsDeleted { get; private set; }

    private Product(string name, string type, double price)
    {
        Name = name;
        Type = type;
        Price = price;
    }

    public static Result<Product> Create(string? name, string? type, double price)
    {
        var validation = TryValidateFields(name, type, price, out var fn, out var tn);
        if (!validation.IsSuccess)
        {
            return Result<Product>.Failure(validation.Error!);
        }

        return Result<Product>.Success(new Product(fn, tn, price));
    }

    public Result Update(string? name, string? type, double price)
    {
        if (!Id.HasValue)
        {
            return Result.Fail("Product has no identity.");
        }

        var validation = TryValidateFields(name, type, price, out var fn, out var tn);
        if (!validation.IsSuccess)
        {
            return validation;
        }

        Name = fn;
        Type = tn;
        Price = price;

        return Result.Ok();
    }

    public Result Delete()
    {
        if (!Id.HasValue)
        {
            return Result.Fail("Product has no identity.");
        }

        if (IsDeleted)
        {
            return Result.Fail("Product is already deleted.");
        }

        IsDeleted = true;

        return Result.Ok();
    }

    private static Result TryValidateFields(string? name, string? type, double price, out string fn, out string tn)
    {
        fn = name?.Trim() ?? string.Empty;
        tn = type?.Trim() ?? string.Empty;

        if (fn.Length == 0)
        {
            return Result.Fail("Name is required.");
        }

        if (fn.Length > MaxNameLength)
        {
            return Result.Fail($"Name cannot exceed {MaxNameLength} characters.");
        }

        if (tn.Length == 0)
        {
            return Result.Fail("Type is required.");
        }

        if (tn.Length > MaxTypeLength)
        {
            return Result.Fail($"Type cannot exceed {MaxTypeLength} characters.");
        }

        if (double.IsNaN(price) || double.IsInfinity(price))
        {
            return Result.Fail("Price must be a valid number.");
        }

        if (price < 0)
        {
            return Result.Fail("Price cannot be negative.");
        }

        return Result.Ok();
    }

    public void SetId(long id)
    {
        if (!Id.HasValue)
        {
            Id = id;
        }
    }
}
