using Bucket.Application.Queries;
using Bucket.Common;
using Bucket.Contract;
using Bucket.Contract.Dtos.Persons;
using MediatR;

namespace Bucket.Application.Handlers.Queries.Persons;

public record GetPersonsQuery(Pagination Pagination) : IRequest<Result<PagedResponse<PersonDTO>>>;

public class GetPersonsQueryHandler : IRequestHandler<GetPersonsQuery, Result<PagedResponse<PersonDTO>>>
{
    private readonly IPersonQuery _personQuery;

    public GetPersonsQueryHandler(IPersonQuery personQuery)
    {
        _personQuery = personQuery;
    }

    public Task<Result<PagedResponse<PersonDTO>>> Handle(GetPersonsQuery request, CancellationToken cancellationToken) =>
        _personQuery.GetPersonsAsync(request.Pagination, cancellationToken);
}
