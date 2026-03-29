using Bucket.Application.Interfaces;
using Bucket.Common;
using Bucket.Domain.Persons;
using DataModel = Bucket.Infrastructure.Data.Data;

namespace Bucket.Infrastructure.Repositories;

public class PersonRepository : IPersonRepository
{
    private readonly DataModel _data;

    public PersonRepository(DataModel data)
    {
        _data = data;
    }

    public Task<Result<long>> AddPersonAsync(Person person, CancellationToken cancellationToken)
    {
        var id = GetNextId();

        person.SetId(id);

        _data.Persons.Add(person);

        return Task.FromResult(Result<long>.Success(id));
    }

    private long GetNextId()
    {
        if (_data.Persons.Count == 0)
        {
            return 1;
        }

        return _data.Persons.Max(p => p.Id!.Value) + 1;
    }
}
