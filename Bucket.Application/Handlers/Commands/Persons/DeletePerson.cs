using Bucket.Application.Interfaces;
using Bucket.Common;
using MediatR;

namespace Bucket.Application.Handlers.Commands.Persons;

public record DeletePersonCommand(long Id) : IRequest<Result<long>>;

public class DeletePersonCommandHandler : IRequestHandler<DeletePersonCommand, Result<long>>
{
    private readonly IPersonRepository _personRepository;

    public DeletePersonCommandHandler(IPersonRepository personRepository)
    {
        _personRepository = personRepository;
    }

    public async Task<Result<long>> Handle(DeletePersonCommand command, CancellationToken cancellationToken)
    {
        var person = await _personRepository.GetByIdAsync(command.Id, cancellationToken);
        if (person is null)
        {
            return Result<long>.Failure("Person not found.");
        }

        var deleteResult = person.Delete();
        if (!deleteResult.IsSuccess)
        {
            return Result<long>.Failure(deleteResult.Error!);
        }

        var saved = await _personRepository.UpdatePersonAsync(person, cancellationToken);
        if (!saved.IsSuccess)
        {
            return Result<long>.Failure(saved.Error!);
        }

        return Result<long>.Success(saved.Value);
    }
}
