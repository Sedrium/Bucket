using Bucket.Common;
using Bucket.Contract;
using Bucket.Domain.Persons;

namespace Bucket.Application.Interfaces;

public interface IPersonQuery
{
    Task<Result<PagedResponse<Person>>> GetPersonsAsync(Pagination pagination, CancellationToken cancellationToken);

    Task<Person?> GetPersonByIdAsync(int id, CancellationToken cancellationToken);
}
