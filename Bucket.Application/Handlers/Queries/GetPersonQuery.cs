using Bucket.Application.Interfaces;
using Bucket.Common;
using Bucket.Contract;
using Bucket.Contract.Persons;
using MediatR;

namespace Bucket.Application.Handlers.Queries;

public record GetPersonQuery(Pagination Pagination) : IRequest<Result<PagedResponse<PersonDto>>>;

public class GetPersonQueryHandler : IRequestHandler<GetPersonQuery, Result<PagedResponse<PersonDto>>>
{
    private readonly IPersonQuery _personQuery;

    public GetPersonQueryHandler(IPersonQuery personQuery)
    {
        _personQuery = personQuery;
    }

    public Task<Result<PagedResponse<PersonDto>>> Handle(GetPersonQuery request, CancellationToken cancellationToken)
    {
        return _personQuery.GetPersonsAsync(request.Pagination, cancellationToken);
    }
}
