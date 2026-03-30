using Bucket.Api.Configuration;
using Bucket.Api.Controllers;
using Bucket.Contract.Dtos.Environment;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace Bucket.Api.Tests.Controllers;

public class EnvironmentControllerTests
{
    [Fact]
    public void Get_ReturnsOk_WithEnvironmentAndFrontend()
    {
        var env = Substitute.For<IWebHostEnvironment>();
        env.EnvironmentName.Returns(Environments.Staging);
        var options = Options.Create(new FrontendOptions { Version = "3.1.0" });
        var subject = new EnvironmentController(env, options);

        var action = subject.Get();

        var ok = Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(action.Result);
        var dto = Assert.IsType<EnvironmentInfoDto>(ok.Value);
        Assert.Equal(Environments.Staging, dto.EnvironmentName);
        Assert.False(dto.IsProduction);
        Assert.Equal("3.1.0", dto.FrontendVersion);
        Assert.False(string.IsNullOrEmpty(dto.ApiVersion));
    }

    [Fact]
    public void Get_WhenProduction_SetsIsProduction()
    {
        var env = Substitute.For<IWebHostEnvironment>();
        env.EnvironmentName.Returns(Environments.Production);
        var options = Options.Create(new FrontendOptions { Version = null });
        var subject = new EnvironmentController(env, options);

        var action = subject.Get();

        var ok = Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(action.Result);
        var dto = Assert.IsType<EnvironmentInfoDto>(ok.Value);
        Assert.True(dto.IsProduction);
    }
}
