using Bucket.Application.Interfaces;
using Bucket.Common;
using Bucket.Domain.Persons;
using MediatR;

namespace Bucket.Application.Handlers.Commands;

public record UpdatePersonCommand(long Id, string Firstname, string Lastname, int YearOfBirth)
    : IRequest<Result<long>>;

public class UpdatePersonCommandHandler : IRequestHandler<UpdatePersonCommand, Result<long>>
{
    private readonly IPersonRepository _personRepository;

    public UpdatePersonCommandHandler(IPersonRepository personRepository)
    {
        _personRepository = personRepository;
    }

    public async Task<Result<long>> Handle(UpdatePersonCommand command, CancellationToken cancellationToken)
    {
        var existing = await _personRepository.GetByIdAsync(command.Id, cancellationToken);
        if (existing is null)
        {
            return Result<long>.Failure("Person not found.");
        }

        var yearOfBirth = YearOfBirth.Create(command.YearOfBirth);
        if (!yearOfBirth.IsSuccess)
        {
            return Result<long>.Failure(yearOfBirth.Error!);
        }

        var changed = existing.Update(command.Firstname, command.Lastname, yearOfBirth.Value!);
        if (!changed.IsSuccess)
        {
            return Result<long>.Failure(changed.Error!);
        }

        var saved = await _personRepository.UpdatePersonAsync(changed.Value!, cancellationToken);
        if (!saved.IsSuccess)
        {
            return Result<long>.Failure(saved.Error!);
        }

        return Result<long>.Success(saved.Value);
    }
}
