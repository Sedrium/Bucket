using Bucket.Application.Interfaces;
using Bucket.Common;
using Bucket.Contract;
using Bucket.Contract.Persons;
using DataModel = Bucket.Infrastructure.Data.Data;

namespace Bucket.Infrastructure.Queries;

public class PersonQuery : IPersonQuery
{
    private readonly DataModel _data;

    public PersonQuery(DataModel data)
    {
        _data = data;
    }

    public async Task<Result<PagedResponse<PersonDto>>> GetPersonsAsync(Pagination pagination, CancellationToken cancellationToken)
    {
        var totalCount = _data.Persons.Count;

        var items = _data.Persons
            .OrderBy(x => x.Id)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .AsEnumerable();

        return Result<PagedResponse<PersonDto>>.Success(new PagedResponse<PersonDto>(items, totalCount));
    }
}
