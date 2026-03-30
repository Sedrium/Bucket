using System.Text;
using Bucket.Api.Controllers;
using Bucket.Api.Tests.TestHelpers;
using Bucket.Application.Handlers.Queries.Reports;
using Bucket.Application.Reports;
using Bucket.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace Bucket.Api.Tests.Controllers;

public class ReportsControllerTests
{
    [Fact]
    public async Task GetPurchaseCsv_WhenNotFound_ReturnsProblem()
    {
        var sender = Substitute.For<ISender>();
        sender.Send(Arg.Any<GetPurchaseCsvReportQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result<PurchaseCsvReport>.NotFound("missing")));
        var subject = new ReportsController(sender);
        ControllerTestContext.Attach(subject);

        var action = await subject.GetPurchaseCsv(1);

        var problem = Assert.IsType<ObjectResult>(action);
        Assert.Equal(404, problem.StatusCode);
    }

    [Fact]
    public async Task GetPurchaseCsv_WhenSuccess_ReturnsFile()
    {
        var report = new PurchaseCsvReport("export.csv", "a;b");
        var sender = Substitute.For<ISender>();
        sender.Send(Arg.Any<GetPurchaseCsvReportQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result<PurchaseCsvReport>.Success(report)));
        var subject = new ReportsController(sender);
        ControllerTestContext.Attach(subject);

        var action = await subject.GetPurchaseCsv(5);

        var file = Assert.IsType<FileContentResult>(action);
        Assert.Equal("text/csv; charset=utf-8", file.ContentType);
        Assert.Equal("export.csv", file.FileDownloadName);
        var expectedBytes = new UTF8Encoding(encoderShouldEmitUTF8Identifier: true).GetBytes("a;b");
        Assert.Equal(expectedBytes, file.FileContents);
    }
}
