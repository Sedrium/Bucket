using Bucket.Application.Interfaces;
using Bucket.Common;
using Bucket.Domain.Persons;
using MediatR;

namespace Bucket.Application.Handlers.Commands;

public record AddPersonCommand(string Firstname, string Lastname, int YearOfBirth)
    : IRequest<Result<long>>;

public class AddPersonCommandHandler : IRequestHandler<AddPersonCommand, Result<long>>
{
    private readonly IPersonRepository _personRepository;

    public AddPersonCommandHandler(IPersonRepository personRepository)
    {
        _personRepository = personRepository;
    }

    public async Task<Result<long>> Handle(AddPersonCommand command, CancellationToken cancellationToken)
    {
        var yearOfBirth = YearOfBirth.Create(command.YearOfBirth);

        if (!yearOfBirth.IsSuccess)
        {
            return Result<long>.Failure(yearOfBirth.Error!);
        }

        var personResult = Person.Create(
            command.Firstname,
            command.Lastname,
            yearOfBirth.Value!);

        if (!personResult.IsSuccess)
        {
            return Result<long>.Failure(personResult.Error!);
        }

        var added = await _personRepository.AddPersonAsync(personResult.Value!, cancellationToken);
        
        if (!added.IsSuccess)
        {
            return Result<long>.Failure(added.Error!);
        }

        var personId = added.Value;

        return Result<long>.Success(personId);
    }
}
