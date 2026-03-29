using Bucket.Contract.Persons;
using Bucket.Domain.Persons;

namespace Bucket.Application;

internal static class PersonMapping
{
    public static PersonResponse ToResponse(this Person person) =>
        new(person.Id, person.FirstName, person.LastName, person.DateOfBirth);
}
