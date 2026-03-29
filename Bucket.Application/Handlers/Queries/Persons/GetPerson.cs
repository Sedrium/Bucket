using Bucket.Application.Interfaces;
using Bucket.Common;
using Bucket.Contract.Dtos.Persons;
using MediatR;

namespace Bucket.Application.Handlers.Queries.Persons;

public record GetPersonQuery(int Id) : IRequest<Result<PersonDTO>>;

public class GetPersonQueryHandler : IRequestHandler<GetPersonQuery, Result<PersonDTO>>
{
    private readonly IPersonQuery _personQuery;

    public GetPersonQueryHandler(IPersonQuery personQuery)
    {
        _personQuery = personQuery;
    }

    public async Task<Result<PersonDTO>> Handle(GetPersonQuery request, CancellationToken cancellationToken)
    {
        var dto = await _personQuery.GetPersonByIdAsync(request.Id, cancellationToken);

        if (dto is null)
        {
            return Result<PersonDTO>.Failure("Person not found.");
        }

        return Result<PersonDTO>.Success(dto);
    }
}
