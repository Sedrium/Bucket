using Bucket.Common;
using Bucket.Domain.Persons;

namespace Bucket.Application.Interfaces;

public interface IPersonRepository
{
    Task<Person?> GetByIdAsync(long id, CancellationToken cancellationToken);

    Task<Result<long>> AddPersonAsync(Person person, CancellationToken cancellationToken);

    Task<Result> UpdatePersonAsync(Person person, CancellationToken cancellationToken);
}
