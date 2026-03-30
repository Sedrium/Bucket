using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Bucket.Api.Tests.TestHelpers;

internal static class ControllerTestContext
{
    public static void Attach(ControllerBase controller)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddControllers();
        services.AddProblemDetails();
        var provider = services.BuildServiceProvider();
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { RequestServices = provider }
        };
    }
}
