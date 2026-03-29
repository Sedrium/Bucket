using Bucket.Common;

namespace Bucket.Domain.Persons;

public sealed class YearOfBirth 
{
    public const int MinimumAgeExclusive = 18;
    public const int MaximumAgeExclusive = 120;

    public DateOnly Value { get; }

    private YearOfBirth(DateOnly value)
    {
        Value = value;
    }

    public static Result<YearOfBirth> Create(int calendarYear)
    {
        DateOnly dateToValid;

        try
        {
            dateToValid = new DateOnly(calendarYear, 1, 1);
        }
        catch (ArgumentOutOfRangeException)
        {
            return Result<YearOfBirth>.Failure("Invalid year.");
        }

        var currentAge = DateOnly.FromDateTime(DateTime.UtcNow).Year - dateToValid.Year;

        if (currentAge <= MinimumAgeExclusive)
        {
            return Result<YearOfBirth>.Failure($"Age must be greater than {MinimumAgeExclusive} years.");
        }

        if (currentAge >= MaximumAgeExclusive)
        {
            return Result<YearOfBirth>.Failure($"Age must be less than {MaximumAgeExclusive} years.");
        }

        return Result<YearOfBirth>.Success(new YearOfBirth(dateToValid));
    }
}
