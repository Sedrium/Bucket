using Bucket.Common;
using Bucket.Domain.Persons;

namespace Bucket.Application.Interfaces;

public interface IPersonRepository
{
    Task<Result<IReadOnlyList<Person>>> AddPersonAsync(Person person, CancellationToken cancellationToken);
}
