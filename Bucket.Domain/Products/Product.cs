namespace Bucket.Domain.Products;

public class Product
{
    public long? Id { get; private set; }
    public string Name { get; }
    public string Type { get; }
    public double Price { get; }

    public Product(string name, string type, double price)
    {
        Name = name;
        Type = type;
        Price = price;
    }

    public void SetId(long id)
    {
        if (!Id.HasValue)
        {
            Id = id;
        }
    }
}
