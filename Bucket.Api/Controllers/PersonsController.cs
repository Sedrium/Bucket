using Bucket.Application.Handlers.Commands;
using Bucket.Application.Handlers.Queries;
using Bucket.Contract;
using Bucket.Contract.Persons;
using MediatR;
using Microsoft.AspNetCore.Http;
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

        [HttpGet]
        public async Task<ActionResult<PagedResponse<PersonResponse>>> GetPersons([FromQuery] GetPersonsRequest request)
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

        [HttpGet("{id:int}")]
        public async Task<ActionResult<PersonResponse>> GetPerson([FromRoute] GetPersonRequest request)
        {
            var result = await _sender.Send(new GetPersonByIdQuery(request.Id));

            if (!result.IsSuccess)
            {
                return Problem(detail: result.Error, statusCode: StatusCodes.Status404NotFound);
            }

            return Ok(result.Value);
        }

        [HttpPost]
        public async Task<ActionResult<IReadOnlyList<PersonResponse>>> AddPerson([FromBody] AddPersonRequest request)
        {
            var result = await _sender.Send(new AddPersonCommand(request));

            if (!result.IsSuccess)
            {
                var statusCode = result.Error == PersonErrors.DuplicateId
                    ? StatusCodes.Status409Conflict
                    : StatusCodes.Status400BadRequest;
                return Problem(detail: result.Error, statusCode: statusCode);
            }

            return Accepted(result.Value);
        }
    }
}
