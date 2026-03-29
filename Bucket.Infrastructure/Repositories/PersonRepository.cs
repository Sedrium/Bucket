using Bucket.Application.Repositories;
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

    public Task<Person?> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        var person = _data.Persons.FirstOrDefault(p => p.Id == id && !p.IsDeleted);
        return Task.FromResult(person);
    }

    public Task<Result<long>> AddPersonAsync(Person person, CancellationToken cancellationToken)
    {
        var id = GetNextId();

        person.SetId(id);

        _data.Persons.Add(person);

        return Task.FromResult(Result<long>.Success(id));
    }

    public Task<Result> UpdatePersonAsync(Person person, CancellationToken cancellationToken)
    {
        var index = _data.Persons.FindIndex(p => p.Id == person.Id);
        if (index < 0)
        {
            return Task.FromResult(Result.NotFound("Person not found."));
        }

        _data.Persons[index] = person;

        return Task.FromResult(Result.Ok());
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
