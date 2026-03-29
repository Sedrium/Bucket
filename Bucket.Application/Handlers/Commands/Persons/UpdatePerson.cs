using Bucket.Application.Repositories;
using Bucket.Common;
using Bucket.Domain.Persons;
using MediatR;

namespace Bucket.Application.Handlers.Commands.Persons;

public record UpdatePersonCommand(long Id, string Firstname, string Lastname, int YearOfBirth)
    : IRequest<Result>;

public class UpdatePersonCommandHandler : IRequestHandler<UpdatePersonCommand, Result>
{
    private readonly IPersonRepository _personRepository;

    public UpdatePersonCommandHandler(IPersonRepository personRepository)
    {
        _personRepository = personRepository;
    }

    public async Task<Result> Handle(UpdatePersonCommand command, CancellationToken cancellationToken)
    {
        var existing = await _personRepository.GetByIdAsync(command.Id, cancellationToken);
        if (existing is null)
        {
            return Result.NotFound("Person not found.");
        }

        var yearOfBirth = YearOfBirth.Create(command.YearOfBirth);
        if (!yearOfBirth.IsSuccess)
        {
            return Result.Fail(yearOfBirth.Error!);
        }

        var updateResult = existing.Update(command.Firstname, command.Lastname, yearOfBirth.Value!);
        if (!updateResult.IsSuccess)
        {
            return Result.Fail(updateResult.Error!);
        }

        var saved = await _personRepository.UpdatePersonAsync(existing, cancellationToken);
        if (!saved.IsSuccess)
        {
            return saved;
        }

        return Result.Ok();
    }
}
