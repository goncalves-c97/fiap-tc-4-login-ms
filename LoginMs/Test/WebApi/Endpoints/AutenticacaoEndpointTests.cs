using Core.Dtos;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WebApi.Endpoints;

namespace Test.WebApi.Endpoints;

public class AutenticacaoEndpointTests
{
    [Fact]
    public void Ctor_WhenMissingSecret_Throws()
    {
        var db = new Moq.Mock<IDbConnection>().Object;
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>()).Build();

        Assert.Throws<ArgumentException>(() => new AutenticacaoEndpoint(db, config));
    }

    [Fact]
    public async Task LoginColaborador_WhenModelStateInvalid_ReturnsBadRequest()
    {
        var db = new Moq.Mock<IDbConnection>().Object;
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
 {
 { "API_AUTHENTICATION_KEY", "0123456789ABCDEF0123456789ABCDEF" }
 }).Build();

        var endpoint = new AutenticacaoEndpoint(db, config);
        endpoint.ModelState.AddModelError("x", "y");

        var result = await endpoint.LoginColaborador(new ColaboradorDto());
        Assert.IsType<BadRequestObjectResult>(result);
    }
}
