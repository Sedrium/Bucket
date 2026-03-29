using Bucket.Application.Interfaces;
using Bucket.Common;
using Bucket.Contract.Persons;
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

    public Task<Result<IReadOnlyList<Person>>> AddPersonAsync(Person person, CancellationToken cancellationToken)
    {
        if (_data.Persons.Exists(p => p.Id == person.Id))
        {
            return Task.FromResult(Result<IReadOnlyList<Person>>.Failure(PersonErrors.DuplicateId));
        }

        _data.Persons.Add(person);
        IReadOnlyList<Person> snapshot = _data.Persons.ToList();
        return Task.FromResult(Result<IReadOnlyList<Person>>.Success(snapshot));
    }
}
