using Bucket.Domain.Persons;

namespace Bucket.Infrastructure.Data;

public sealed class Data
{
    public static Data Instance { get; } = new();

    private Data()
    {
    }

    public List<Person> Persons { get; } =
    [
        MustCreate(1, "John", "Doe", new DateOnly(1980, 6, 15)),
        MustCreate(2, "Jane", "Doe", new DateOnly(1985, 3, 22)),
        MustCreate(3, "Bob", "Smith", new DateOnly(1990, 11, 8)),
        MustCreate(4, "Alice", "Johnson", new DateOnly(1995, 1, 30)),
        MustCreate(5, "Mike", "Brown", new DateOnly(1982, 9, 5)),
        MustCreate(6, "Samantha", "Davis", new DateOnly(1987, 7, 19)),
        MustCreate(7, "David", "Wilson", new DateOnly(1992, 4, 12)),
        MustCreate(8, "Emily", "Taylor", new DateOnly(1997, 12, 1)),
        MustCreate(9, "Chris", "Anderson", new DateOnly(1984, 2, 28)),
        MustCreate(10, "Jessica", "Thomas", new DateOnly(1989, 8, 14))
    ];

    private static Person MustCreate(double id, string firstName, string lastName, DateOnly dateOfBirth)
    {
        var result = Person.Create(id, firstName, lastName, dateOfBirth);
        if (!result.IsSuccess)
        {
            throw new InvalidOperationException($"Seed person invalid: {result.Error}");
        }

        return result.Value!;
    }
}
