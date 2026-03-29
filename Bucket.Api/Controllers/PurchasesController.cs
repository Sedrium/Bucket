using Bucket.Application.Handlers.Commands.Purchases;
using Bucket.Application.Handlers.Queries.Purchases;
using Bucket.Contract;
using Bucket.Contract.Dtos.Purchases;
using Bucket.Contract.Purchases;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bucket.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class PurchasesController : ControllerBase
{
    private readonly ISender _sender;

    public PurchasesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<PurchaseDTO>>> GetPurchases([FromQuery] GetPurchasesRequest request)
    {
        var paginationResult = Pagination.Create(request.Page, request.PageSize);

        if (!paginationResult.IsSuccess)
        {
            return Problem(detail: paginationResult.Error, statusCode: StatusCodes.Status400BadRequest);
        }

        var result = await _sender.Send(new GetPurchasesQuery(paginationResult.Value!));

        if (!result.IsSuccess)
        {
            return Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
        }

        return Ok(result.Value);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<PurchaseDTO>> GetPurchase([FromRoute] GetPurchaseRequest request)
    {
        var result = await _sender.Send(new GetPurchaseQuery(request.Id));

        if (!result.IsSuccess)
        {
            return Problem(detail: result.Error, statusCode: StatusCodes.Status404NotFound);
        }

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<ActionResult<long>> AddPurchase([FromBody] AddPurchaseRequest request)
    {
        var result = await _sender.Send(new AddPurchaseCommand(request.CustomerId, request.ProductIds));

        if (!result.IsSuccess)
        {
            var notFound = result.Error == "Customer not found."
                || (result.Error?.StartsWith("Product with id", StringComparison.Ordinal) ?? false);
            var status = notFound ? StatusCodes.Status404NotFound : StatusCodes.Status400BadRequest;

            return Problem(detail: result.Error, statusCode: status);
        }

        return Accepted(result.Value);
    }

    [HttpDelete("by-customer/{customerId:long}")]
    public async Task<ActionResult<int>> DeletePurchasesByCustomer([FromRoute] CustomerIdRouteRequest route)
    {
        var result = await _sender.Send(new DeletePurchasesByCustomerCommand(route.CustomerId));

        if (!result.IsSuccess)
        {
            var status = result.Error is "Customer not found." or "No active purchases for this customer."
                ? StatusCodes.Status404NotFound
                : StatusCodes.Status400BadRequest;
            return Problem(detail: result.Error, statusCode: status);
        }

        return Accepted(result.Value);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeletePurchase([FromRoute] PurchaseIdRouteRequest route)
    {
        var result = await _sender.Send(new DeletePurchaseCommand(route.Id));

        if (!result.IsSuccess)
        {
            var status = result.Error == "Purchase not found."
                ? StatusCodes.Status404NotFound
                : StatusCodes.Status400BadRequest;
            return Problem(detail: result.Error, statusCode: status);
        }

        return Accepted();
    }
}
