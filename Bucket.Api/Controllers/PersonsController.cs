using Bucket.Application.Handlers.Queries;
using Bucket.Contract;
using Bucket.Contract.Persons;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Bucket.Api.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class PersonsController : ControllerBase
    {
        private readonly ISender _sender;

        public PersonsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet()]
        public async Task<ActionResult<PagedResponse<PersonDto>>> GetPersons([FromQuery] GetPersonsRequest request)
        {
            var paginationResult = Pagination.Create(request.Page, request.PageSize);

            if (!paginationResult.IsSuccess)
            {
                return Problem(detail: paginationResult.Error, statusCode: StatusCodes.Status400BadRequest);
            }

            var result = await _sender.Send(new GetPersonQuery(paginationResult.Value!));

            if (!result.IsSuccess)
            {
                return Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
            }

            return Ok(result.Value);
        }
    }
}
