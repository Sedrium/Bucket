using Bucket.Common;

namespace Bucket.Domain.Persons;

public class Person
{
    private const int MaxNameLength = 100;

    public long? Id { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public YearOfBirth AgeYears { get; private set; }
    public bool IsDeleted { get; private set; }

    private Person(string firstName, string lastName, YearOfBirth yearOfBirth)
    {
        FirstName = firstName;
        LastName = lastName;
        AgeYears = yearOfBirth;
    }

    public static Result<Person> Create(string? firstName, string? lastName, YearOfBirth yearOfBirth)
    {
        var validation = TryValidateFields(firstName, lastName, yearOfBirth);
        if (!validation.IsSuccess)
        {
            return Result<Person>.Failure(validation.Error!);
        }

        return Result<Person>.Success(new Person(firstName, lastName, yearOfBirth));
    }

    public Result Update(string? firstName, string? lastName, YearOfBirth yearOfBirth)
    {
        if (!Id.HasValue)
        {
            return Result.Fail("Person has no identity.");
        }

        var validation = TryValidateFields(firstName, lastName, yearOfBirth);
        if (!validation.IsSuccess)
        {
            return validation;
        }

        FirstName = firstName;
        LastName = lastName;
        AgeYears = yearOfBirth;

        return Result.Ok();
    }

    public Result Delete()
    {
        if (!Id.HasValue)
        {
            return Result.Fail("Person has no identity.");
        }

        if (IsDeleted)
        {
            return Result.Fail("Person is already deleted.");
        }

        IsDeleted = true;

        return Result.Ok();
    }

    private static Result TryValidateFields(string? firstName, string? lastName, YearOfBirth? yearOfBirth)
    {
        if (yearOfBirth is null)
        {
            return Result.Fail("Year of birth is required.");
        }

        var fn = firstName?.Trim() ?? string.Empty;
        var ln = lastName?.Trim() ?? string.Empty;

        if (fn.Length == 0)
        {
            return Result.Fail("First name is required.");
        }

        if (fn.Length > MaxNameLength)
        {
            return Result.Fail($"First name cannot exceed {MaxNameLength} characters.");
        }

        if (ln.Length == 0)
        {
            return Result.Fail("Last name is required.");
        }

        if (ln.Length > MaxNameLength)
        {
            return Result.Fail($"Last name cannot exceed {MaxNameLength} characters.");
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
