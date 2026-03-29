using Bucket.Application.Handlers.Commands;
using Bucket.Application.Handlers.Queries;
using Bucket.Contract;
using Bucket.Contract.Dtos.Persons;
using Bucket.Contract.Persons.Persons;
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
        public async Task<ActionResult<PagedResponse<PersonDTO>>> GetPersons([FromQuery] GetPersonsRequest request)
        {
            var paginationResult = Pagination.Create(request.Page, request.PageSize);

            if (!paginationResult.IsSuccess)
            {
                return Problem(detail: paginationResult.Error, statusCode: StatusCodes.Status400BadRequest);
            }

            var result = await _sender.Send(new GetPersonsQuery(paginationResult.Value!));

            if (!result.IsSuccess)
            {
                return Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
            }

            return Ok(result.Value);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<PersonDTO>> GetPerson([FromRoute] GetPersonRequest request)
        {
            var result = await _sender.Send<Common.Result<PersonDTO>>(new GetPersonQuery(request.Id));

            if (!result.IsSuccess)
            {
                return Problem(detail: result.Error, statusCode: StatusCodes.Status404NotFound);
            }

            return Ok(result.Value);
        }

        [HttpPost]
        public async Task<ActionResult<long>> AddPerson([FromBody] AddPersonRequest request)
        {
            var result = await _sender.Send(new AddPersonCommand(request.Firstname, request.Lastname, request.DateOfBirth));

            if (!result.IsSuccess)
            {
                return Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
            }

            return Accepted(result.Value);
        }

        [HttpPut("{id:long}")]
        public async Task<ActionResult<long>> UpdatePerson(long id, [FromBody] UpdatePersonRequest request)
        {
            var result = await _sender.Send(new UpdatePersonCommand(id, request.Firstname, request.Lastname, request.DateOfBirth));

            if (!result.IsSuccess)
            {
                var status = result.Error == "Person not found."
                    ? StatusCodes.Status404NotFound
                    : StatusCodes.Status400BadRequest;
                return Problem(detail: result.Error, statusCode: status);
            }

            return Ok(result.Value);
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> DeletePerson(long id)
        {
            var result = await _sender.Send(new DeletePersonCommand(id));

            if (!result.IsSuccess)
            {
                var status = result.Error == "Person not found."
                    ? StatusCodes.Status404NotFound
                    : StatusCodes.Status400BadRequest;
                return Problem(detail: result.Error, statusCode: status);
            }

            return Accepted();
        }
    }
}
