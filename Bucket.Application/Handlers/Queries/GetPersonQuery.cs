using Bucket.Application.Interfaces;
using Bucket.Common;
using Bucket.Contract;
using Bucket.Contract.Persons;
using MediatR;

namespace Bucket.Application.Handlers.Queries;

public record GetPersonQuery(Pagination Pagination) : IRequest<Result<PagedResponse<PersonResponse>>>;

public class GetPersonQueryHandler : IRequestHandler<GetPersonQuery, Result<PagedResponse<PersonResponse>>>
{
    private readonly IPersonQuery _personQuery;

    public GetPersonQueryHandler(IPersonQuery personQuery)
    {
        _personQuery = personQuery;
    }

    public async Task<Result<PagedResponse<PersonResponse>>> Handle(GetPersonQuery request, CancellationToken cancellationToken)
    {
        var result = await _personQuery.GetPersonsAsync(request.Pagination, cancellationToken);

        if (!result.IsSuccess)
        {
            return Result<PagedResponse<PersonResponse>>.Failure(result.Error!);
        }

        var mapped = result.Value!.Items.Select(p => p.ToResponse()).ToList();
        
        return Result<PagedResponse<PersonResponse>>.Success(new PagedResponse<PersonResponse>(mapped, result.Value.TotalCount));
    }
}
