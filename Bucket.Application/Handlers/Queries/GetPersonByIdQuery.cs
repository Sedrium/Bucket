using Bucket.Application;
using Bucket.Application.Interfaces;
using Bucket.Common;
using Bucket.Contract.Persons;
using MediatR;

namespace Bucket.Application.Handlers.Queries;

public record GetPersonByIdQuery(int Id) : IRequest<Result<PersonResponse>>;

public class GetPersonByIdQueryHandler : IRequestHandler<GetPersonByIdQuery, Result<PersonResponse>>
{
    private readonly IPersonQuery _personQuery;

    public GetPersonByIdQueryHandler(IPersonQuery personQuery)
    {
        _personQuery = personQuery;
    }

    public async Task<Result<PersonResponse>> Handle(GetPersonByIdQuery request, CancellationToken cancellationToken)
    {
        var person = await _personQuery.GetPersonByIdAsync(request.Id, cancellationToken);

        if (person is null)
        {
            return Result<PersonResponse>.Failure("Person not found.");
        }

        return Result<PersonResponse>.Success(person.ToResponse());
    }
}
