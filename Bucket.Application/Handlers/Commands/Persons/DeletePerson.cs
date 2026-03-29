using Bucket.Application.Repositories;
using Bucket.Common;
using MediatR;

namespace Bucket.Application.Handlers.Commands.Persons;

public record DeletePersonCommand(long Id) : IRequest<Result>;

public class DeletePersonCommandHandler : IRequestHandler<DeletePersonCommand, Result>
{
    private readonly IPersonRepository _personRepository;

    public DeletePersonCommandHandler(IPersonRepository personRepository)
    {
        _personRepository = personRepository;
    }

    public async Task<Result> Handle(DeletePersonCommand command, CancellationToken cancellationToken)
    {
        var person = await _personRepository.GetByIdAsync(command.Id, cancellationToken);
        if (person is null)
        {
            return Result.NotFound("Person not found.");
        }

        var deleteResult = person.Delete();
        if (!deleteResult.IsSuccess)
        {
            return deleteResult;
        }

        var saved = await _personRepository.UpdatePersonAsync(person, cancellationToken);
        if (!saved.IsSuccess)
        {
            return saved;
        }

        return Result.Ok();
    }
}
