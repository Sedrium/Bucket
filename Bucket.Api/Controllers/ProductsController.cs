using Bucket.Api.Http;
using Bucket.Application.Handlers.Commands.Products;
using Bucket.Application.Handlers.Queries.Products;
using Bucket.Contract;
using Bucket.Contract.Dtos.Products;
using Bucket.Contract.Products;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bucket.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ISender _sender;

    public ProductsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<ProductDTO>>> GetProducts([FromQuery] GetProductsRequest request)
    {
        var paginationResult = Pagination.Create(request.Page, request.PageSize);

        if (!paginationResult.IsSuccess)
        {
            return Problem(
                detail: paginationResult.Error,
                statusCode: paginationResult.GetStatusCode());
        }

        var result = await _sender.Send(new GetProductsQuery(paginationResult.Value!));

        if (!result.IsSuccess)
        {
            return Problem(detail: result.Error, statusCode: result.GetStatusCode());
        }

        return Ok(result.Value);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ProductDTO>> GetProduct([FromRoute] GetProductRequest request)
    {
        var result = await _sender.Send(new GetProductQuery(request.Id));

        if (!result.IsSuccess)
        {
            return Problem(detail: result.Error, statusCode: result.GetStatusCode());
        }

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<ActionResult<long>> AddProduct([FromBody] AddProductRequest request)
    {
        var result = await _sender.Send(new AddProductCommand(request.Name, request.Type, request.Price));

        if (!result.IsSuccess)
        {
            return Problem(detail: result.Error, statusCode: result.GetStatusCode());
        }

        return Accepted(result.Value);
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<long>> UpdateProduct([FromRoute] ProductIdRouteRequest route, [FromBody] UpdateProductRequest request)
    {
        var result = await _sender.Send(new UpdateProductCommand(route.Id, request.Name, request.Type, request.Price));

        if (!result.IsSuccess)
        {
            return Problem(detail: result.Error, statusCode: result.GetStatusCode());
        }

        return Ok(result.Value);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteProduct([FromRoute] ProductIdRouteRequest route)
    {
        var result = await _sender.Send(new DeleteProductCommand(route.Id));

        if (!result.IsSuccess)
        {
            return Problem(detail: result.Error, statusCode: result.GetStatusCode());
        }

        return Accepted();
    }
}
