using Bucket.Application.Repositories;
using Bucket.Common;
using Bucket.Domain.Persons;
using MediatR;

namespace Bucket.Application.Handlers.Commands.Persons;

public record AddPersonCommand(string Firstname, string Lastname, int YearOfBirth)
    : IRequest<Result<EntityId>>;

public class AddPersonCommandHandler : IRequestHandler<AddPersonCommand, Result<EntityId>>
{
    private readonly IPersonRepository _personRepository;

    public AddPersonCommandHandler(IPersonRepository personRepository)
    {
        _personRepository = personRepository;
    }

    public async Task<Result<EntityId>> Handle(AddPersonCommand command, CancellationToken cancellationToken)
    {
        var yearOfBirth = YearOfBirth.Create(command.YearOfBirth);

        if (!yearOfBirth.IsSuccess)
        {
            return Result<EntityId>.Failure(yearOfBirth.Error!);
        }

        var personResult = Person.Create(
            command.Firstname,
            command.Lastname,
            yearOfBirth.Value!);

        if (!personResult.IsSuccess)
        {
            return Result<EntityId>.Failure(personResult.Error!);
        }

        var added = await _personRepository.AddPersonAsync(personResult.Value!, cancellationToken);

        if (!added.IsSuccess)
        {
            return added.FailureKind == ResultFailureKind.NotFound
                ? Result<EntityId>.NotFound(added.Error!)
                : Result<EntityId>.Failure(added.Error!);
        }

        return Result<EntityId>.Success(new EntityId(added.Value));
    }
}
