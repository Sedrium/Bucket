using Bucket.Common;
using Bucket.Contract;
using Bucket.Contract.Persons;

namespace Bucket.Application.Interfaces;

public interface IPersonQuery
{
    Task<Result<PagedResponse<PersonDto>>> GetPersonsAsync(Pagination pagination, CancellationToken cancellationToken);
}
