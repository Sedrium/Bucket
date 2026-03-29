using System.Reflection;
using Bucket.Api.Configuration;
using Bucket.Contract.Dtos.Environment;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Bucket.Api.Controllers;

[ApiController]
[Route("environment")]
public class EnvironmentController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;
    private readonly FrontendOptions _frontend;

    public EnvironmentController(
        IWebHostEnvironment environment,
        IOptions<FrontendOptions> frontendOptions)
    {
        _environment = environment;
        _frontend = frontendOptions.Value;
    }

    [HttpGet]
    public ActionResult<EnvironmentInfoDto> Get()
    {
        var assembly = typeof(Program).Assembly;
        var apiVersion =
            assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
            ?? assembly.GetName().Version?.ToString()
            ?? "unknown";

        var response = new EnvironmentInfoDto(
            EnvironmentName: _environment.EnvironmentName,
            IsProduction: _environment.IsProduction(),
            ApiVersion: apiVersion,
            FrontendVersion: _frontend.Version);

        return Ok(response);
    }
}
