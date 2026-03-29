using Bucket.Common;

namespace Bucket.Domain.Persons;

public class Person
{
    private const int MaxNameLength = 100;

    public long? Id { get; private set; }
    public string FirstName { get; }
    public string LastName { get; }
    public YearOfBirth AgeYears { get; }

    private Person(string firstName, string lastName, YearOfBirth yearOfBirth)
    {
        FirstName = firstName;
        LastName = lastName;
        AgeYears = yearOfBirth;
    }

    public static Result<Person> Create(string? firstName, string? lastName, YearOfBirth yearOfBirth)
    {
        var fn = firstName?.Trim() ?? string.Empty;
        var ln = lastName?.Trim() ?? string.Empty;

        if (fn.Length == 0)
        {
            return Result<Person>.Failure("First name is required.");
        }

        if (fn.Length > MaxNameLength)
        {
            return Result<Person>.Failure($"First name cannot exceed {MaxNameLength} characters.");
        }

        if (ln.Length == 0)
        {
            return Result<Person>.Failure("Last name is required.");
        }

        if (ln.Length > MaxNameLength)
        {
            return Result<Person>.Failure($"Last name cannot exceed {MaxNameLength} characters.");
        }

        if (yearOfBirth == default)
        {
            return Result<Person>.Failure("Year of birth is required.");
        }

        return Result<Person>.Success(new Person(fn, ln, yearOfBirth));
    }

    public void SetId(long id)
    {
        if (!Id.HasValue)
        {
            Id = id;
        }
    }
}
