using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Test.Helpers.Fakes;
using WebApi.Endpoints;

namespace Test.WebApi.Endpoints;

public class SetupEndpointsTests
{
    [Fact]
    public void Ctor_WhenMissingSecret_Throws()
    {
        var db = new Moq.Mock<IDbConnection>().Object;
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>()).Build();

        Assert.Throws<ArgumentException>(() => new SetupEndpoint(db, config));
    }

    [Fact]
    public async Task ResetClientes_ReturnsOk()
    {
        var db = new FakeDbConnection();
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "API_AUTHENTICATION_KEY", "0123456789ABCDEF0123456789ABCDEF" }
        }).Build();

        var endpoint = new SetupEndpoint(db, config);

        var result = await endpoint.ResetClientes();

        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task ResetColaboradores_ReturnsOk()
    {
        var db = new FakeDbConnection();
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "API_AUTHENTICATION_KEY", "0123456789ABCDEF0123456789ABCDEF" }
        }).Build();

        var endpoint = new SetupEndpoint(db, config);

        var result = await endpoint.ResetColaboradores();

        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task SetupColaboradoresMock_ReturnsOk()
    {
        var db = new FakeDbConnection();
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "API_AUTHENTICATION_KEY", "0123456789ABCDEF0123456789ABCDEF" }
        }).Build();

        var endpoint = new SetupEndpoint(db, config);

        var result = await endpoint.SetupColaboradoresMock();

        Assert.IsType<OkResult>(result);
        Assert.NotEmpty(db.InsertAndReturnIds);
    }

    [Fact]
    public async Task LoginAdministradorMock_WhenModelStateInvalid_ReturnsBadRequest()
    {
        var db = new FakeDbConnection();
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "API_AUTHENTICATION_KEY", "0123456789ABCDEF0123456789ABCDEF" }
        }).Build();

        var endpoint = new SetupEndpoint(db, config);
        endpoint.ModelState.AddModelError("x", "y");

        var result = await endpoint.LoginAdministradorMock();

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task LoginCozinheiroMock_WhenModelStateInvalid_ReturnsBadRequest()
    {
        var db = new FakeDbConnection();
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "API_AUTHENTICATION_KEY", "0123456789ABCDEF0123456789ABCDEF" }
        }).Build();

        var endpoint = new SetupEndpoint(db, config);
        endpoint.ModelState.AddModelError("x", "y");

        var result = await endpoint.LoginCozinheiroMock();

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task LoginAtendenteMock_WhenModelStateInvalid_ReturnsBadRequest()
    {
        var db = new FakeDbConnection();
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "API_AUTHENTICATION_KEY", "0123456789ABCDEF0123456789ABCDEF" }
        }).Build();

        var endpoint = new SetupEndpoint(db, config);
        endpoint.ModelState.AddModelError("x", "y");

        var result = await endpoint.LoginAtendenteMock();

        Assert.IsType<BadRequestObjectResult>(result);
    }
}
