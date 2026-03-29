using System.Text;
using Bucket.Api.Http;
using Bucket.Application.Handlers.Queries.Reports;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Bucket.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ReportsController : ControllerBase
{
    private readonly ISender _sender;
    private static readonly UTF8Encoding Utf8WithBom = new(encoderShouldEmitUTF8Identifier: true);

    public ReportsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("purchases/{purchaseId:long}/csv")]
    public async Task<IActionResult> GetPurchaseCsv([FromRoute] long purchaseId)
    {
        var result = await _sender.Send(new GetPurchaseCsvReportQuery(purchaseId));

        if (!result.IsSuccess)
        {
            return Problem(detail: result.Error, statusCode: result.GetStatusCode());
        }

        var report = result.Value!;

        var bytes = Utf8WithBom.GetBytes(report.Content);

        return File(bytes, "text/csv; charset=utf-8", report.FileName);
    }
}
