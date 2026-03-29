using Bucket.Contract.Persons;

namespace Bucket.Infrastructure.Data;

/// <summary>
/// Temporary in-memory data source used by query implementations.
/// </summary>
public sealed class Data
{
    public static Data Instance { get; } = new();

    private Data()
    {
    }

    public List<PersonDto> Persons { get; } =
    [
        new(1, "John", "Doe", 1980),
        new(2, "Jane", "Doe", 1985),
        new(3, "Bob", "Smith", 1990),
        new(4, "Alice", "Johnson", 1995),
        new(5, "Mike", "Brown", 1982),
        new(6, "Samantha", "Davis", 1987),
        new(7, "David", "Wilson", 1992),
        new(8, "Emily", "Taylor", 1997),
        new(9, "Chris", "Anderson", 1984),
        new(10, "Jessica", "Thomas", 1989)
    ];
}
