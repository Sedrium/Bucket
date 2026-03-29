using Bucket.Application.Interfaces;
using Bucket.Common;
using Bucket.Contract;
using Bucket.Domain.Persons;
using DataModel = Bucket.Infrastructure.Data.Data;

namespace Bucket.Infrastructure.Queries;

public class PersonQuery : IPersonQuery
{
    private readonly DataModel _data;

    public PersonQuery(DataModel data)
    {
        _data = data;
    }

    public Task<Result<PagedResponse<Person>>> GetPersonsAsync(Pagination pagination, CancellationToken cancellationToken)
    {
        var totalCount = _data.Persons.Count;

        var items = _data.Persons
            .OrderBy(x => x.Id)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .AsEnumerable();

        return Task.FromResult(Result<PagedResponse<Person>>.Success(new PagedResponse<Person>(items, totalCount)));
    }

    public Task<Person?> GetPersonByIdAsync(int id, CancellationToken cancellationToken)
    {
        var person = _data.Persons.FirstOrDefault(p => p.Id == id);
        return Task.FromResult(person);
    }
}
