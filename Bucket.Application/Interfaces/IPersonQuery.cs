using Bucket.Common;
using Bucket.Contract;
using Bucket.Contract.Dtos.Persons;

namespace Bucket.Application.Interfaces;

public interface IPersonQuery
{
    Task<Result<PagedResponse<PersonDTO>>> GetPersonsAsync(Pagination pagination, CancellationToken cancellationToken);

    Task<PersonDTO?> GetPersonByIdAsync(int id, CancellationToken cancellationToken);
}
