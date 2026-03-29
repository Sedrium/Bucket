using Bucket.Common;

namespace Bucket.Domain.Persons;

public class Person
{
    public const int MaxNameLength = 100;

    public double Id { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public DateOnly DateOfBirth { get; }

    private Person(double id, string firstName, string lastName, DateOnly dateOfBirth)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
    }

    public static Result<Person> Create(double id, string? firstName, string? lastName, DateOnly dateOfBirth)
    {
        if (id < 1)
        {
            return Result<Person>.Failure("Id must be at least 1.");
        }

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

        if (dateOfBirth == default)
        {
            return Result<Person>.Failure("Date of birth is required.");
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        if (dateOfBirth > today)
        {
            return Result<Person>.Failure("Date of birth cannot be in the future.");
        }

        if (dateOfBirth < today.AddYears(-150))
        {
            return Result<Person>.Failure("Date of birth is not valid.");
        }

        return Result<Person>.Success(new Person(id, fn, ln, dateOfBirth));
    }
}
