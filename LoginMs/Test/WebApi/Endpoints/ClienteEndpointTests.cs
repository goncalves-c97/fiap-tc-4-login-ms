using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using WebApi.Endpoints;

namespace Test.WebApi.Endpoints;

public class ClienteEndpointTests
{
    [Fact]
    public async Task GetByCpf_WhenNotFound_ReturnsNotFound()
    {
        var db = new Moq.Mock<IDbConnection>().Object;
        var endpoint = new ClienteEndpoint(db);

        var result = await endpoint.GetByCpf("123");

        Assert.IsType<NotFoundObjectResult>(result);
    }
}
