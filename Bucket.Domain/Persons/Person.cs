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

    public static Result<Person> Create(string? firstName, string? lastName, YearOfBirth yearOfBirth) =>
        Build(firstName, lastName, yearOfBirth);

    public Result<Person> Update(string? firstName, string? lastName, YearOfBirth yearOfBirth)
    {
        if (!Id.HasValue)
        {
            return Result<Person>.Failure("Person has no identity.");
        }

        return Build(firstName, lastName, yearOfBirth);
    }

    private static Result<Person> Build(string? firstName, string? lastName, YearOfBirth? yearOfBirth)
    {
        if (yearOfBirth is null)
        {
            return Result<Person>.Failure("Year of birth is required.");
        }

        var names = ValidateNames(firstName, lastName);
        if (!names.IsSuccess)
        {
            return Result<Person>.Failure(names.Error!);
        }

        return Result<Person>.Success(new Person(names.Value.FirstName, names.Value.LastName, yearOfBirth));
    }

    private static Result<(string FirstName, string LastName)> ValidateNames(string? firstName, string? lastName)
    {
        var fn = firstName?.Trim() ?? string.Empty;
        var ln = lastName?.Trim() ?? string.Empty;

        if (fn.Length == 0)
        {
            return Result<(string, string)>.Failure("First name is required.");
        }

        if (fn.Length > MaxNameLength)
        {
            return Result<(string, string)>.Failure($"First name cannot exceed {MaxNameLength} characters.");
        }

        if (ln.Length == 0)
        {
            return Result<(string, string)>.Failure("Last name is required.");
        }

        if (ln.Length > MaxNameLength)
        {
            return Result<(string, string)>.Failure($"Last name cannot exceed {MaxNameLength} characters.");
        }

        return Result<(string, string)>.Success((fn, ln));
    }

    public void SetId(long id)
    {
        if (!Id.HasValue)
        {
            Id = id;
        }
    }
}
