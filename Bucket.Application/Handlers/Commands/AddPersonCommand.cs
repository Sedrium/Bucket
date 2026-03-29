using Bucket.Application;
using Bucket.Application.Interfaces;
using Bucket.Common;
using Bucket.Contract.Persons;
using Bucket.Domain.Persons;
using MediatR;

namespace Bucket.Application.Handlers.Commands;

public record AddPersonCommand(AddPersonRequest Body) : IRequest<Result<IReadOnlyList<PersonResponse>>>;

public class AddPersonCommandHandler : IRequestHandler<AddPersonCommand, Result<IReadOnlyList<PersonResponse>>>
{
    private readonly IPersonRepository _personRepository;

    public AddPersonCommandHandler(IPersonRepository personRepository)
    {
        _personRepository = personRepository;
    }

    public async Task<Result<IReadOnlyList<PersonResponse>>> Handle(AddPersonCommand request, CancellationToken cancellationToken)
    {
        var created = Person.Create(request.Body.Id, request.Body.Firstname, request.Body.Lastname, request.Body.DateOfBirth);
        if (!created.IsSuccess)
        {
            return Result<IReadOnlyList<PersonResponse>>.Failure(created.Error!);
        }

        var added = await _personRepository.AddPersonAsync(created.Value!, cancellationToken);
        if (!added.IsSuccess)
        {
            return Result<IReadOnlyList<PersonResponse>>.Failure(added.Error!);
        }

        var responses = added.Value!.Select(p => p.ToResponse()).ToList();
        return Result<IReadOnlyList<PersonResponse>>.Success(responses);
    }
}
