using Bucket.Common;

namespace Bucket.Domain.Persons;

public class Person
{
    private const int MaxNameLength = 100;

    public long? Id { get; private set; }
    public string FirstName { get; }
    public string LastName { get; }
    public YearOfBirth AgeYears { get; }
    public bool IsDeleted { get; }

    private Person(string firstName, string lastName, YearOfBirth yearOfBirth, long? id, bool isDeleted)
    {
        FirstName = firstName;
        LastName = lastName;
        AgeYears = yearOfBirth;
        Id = id;
        IsDeleted = isDeleted;
    }

    public static Result<Person> Create(string? firstName, string? lastName, YearOfBirth yearOfBirth) =>
        Build(firstName, lastName, yearOfBirth, null, false);

    public Result<Person> Update(string? firstName, string? lastName, YearOfBirth yearOfBirth)
    {
        if (!Id.HasValue)
        {
            return Result<Person>.Failure("Person has no identity.");
        }

        return Build(firstName, lastName, yearOfBirth, Id.Value, IsDeleted);
    }

    public Result<Person> Delete()
    {
        if (!Id.HasValue)
        {
            return Result<Person>.Failure("Person has no identity.");
        }

        if (IsDeleted)
        {
            return Result<Person>.Failure("Person is already deleted.");
        }

        return Result<Person>.Success(this);
    }

    private static Result<Person> Build(string? firstName, string? lastName, YearOfBirth? yearOfBirth, long? id, bool isDeleted)
    {
        if (yearOfBirth is null)
        {
            return Result<Person>.Failure("Year of birth is required.");
        }

        if (id.HasValue && id.Value < 1)
        {
            return Result<Person>.Failure("Id must be at least 1.");
        }

        var names = ValidateNames(firstName, lastName);
        if (!names.IsSuccess)
        {
            return Result<Person>.Failure(names.Error!);
        }

        var n = names.Value!;
        return Result<Person>.Success(new Person(n.FirstName, n.LastName, yearOfBirth, id, isDeleted));
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
