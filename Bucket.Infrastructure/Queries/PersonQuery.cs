using Bucket.Application.Interfaces;
using Bucket.Common;
using Bucket.Contract;
using Bucket.Contract.Dtos.Persons;
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

    public Task<Result<PagedResponse<PersonDTO>>> GetPersonsAsync(Pagination pagination, CancellationToken cancellationToken)
    {
        var active = _data.Persons.Where(p => !p.IsDeleted).OrderBy(x => x.Id).ToList();
        var totalCount = active.Count;

        var items = active
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(MapToDto)
            .ToList();

        return Task.FromResult(Result<PagedResponse<PersonDTO>>.Success(new PagedResponse<PersonDTO>(items, totalCount)));
    }

    public Task<PersonDTO?> GetPersonByIdAsync(int id, CancellationToken cancellationToken)
    {
        var person = _data.Persons.FirstOrDefault(p => p.Id == id && !p.IsDeleted);
        return Task.FromResult(person is null ? null : MapToDto(person));
    }

    private static PersonDTO MapToDto(Person person)
    {
        if (!person.Id.HasValue)
        {
            throw new InvalidOperationException("Person id is required for mapping.");
        }

        return new PersonDTO(person.Id.Value, person.FirstName, person.LastName, person.AgeYears.Value.Year);
    }
}
