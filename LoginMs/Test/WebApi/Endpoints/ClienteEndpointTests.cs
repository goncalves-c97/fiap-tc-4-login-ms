using Core.Dtos;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Test.Helpers.Fakes;
using WebApi.Endpoints;

namespace Test.WebApi.Endpoints;

public class ClienteEndpointTests
{
    [Fact]
    public async Task GetByCpf_WhenNotFound_ReturnsNotFound()
    {
        var db = new FakeDbConnection
        {
            SearchFirstOrDefaultHandler = (_, __, ___) => null
        };
        var endpoint = new ClienteEndpoint(db);

        var result = await endpoint.GetByCpf("123");

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Cliente não encontrado!", notFound.Value);
    }

    [Fact]
    public async Task GetByCpf_WhenFound_ReturnsOkWithCliente()
    {
        var expected = new Cliente { IdCliente =10, Cpf = "123", Nome = "Teste", Email = "a@b.com" };

        var db = new FakeDbConnection
        {
            SearchFirstOrDefaultHandler = (_, __, ___) => expected
        };
        var endpoint = new ClienteEndpoint(db);

        var result = await endpoint.GetByCpf("123");

        var ok = Assert.IsType<OkObjectResult>(result);
        var cliente = Assert.IsType<Cliente>(ok.Value);
        Assert.Equal(expected.IdCliente, cliente.IdCliente);
        Assert.Equal(expected.Cpf, cliente.Cpf);
    }

    [Fact]
    public async Task GetByEmail_WhenNotFound_ReturnsNotFound()
    {
        var db = new FakeDbConnection
        {
            SearchFirstOrDefaultHandler = (_, __, ___) => null
        };
        var endpoint = new ClienteEndpoint(db);

        var result = await endpoint.GetByEmail("x@y.com");

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Cliente não encontrado!", notFound.Value);
    }

    [Fact]
    public async Task GetByEmail_WhenFound_ReturnsOkWithCliente()
    {
        var expected = new Cliente { IdCliente =11, Cpf = "999", Nome = "Email", Email = "x@y.com" };

        var db = new FakeDbConnection
        {
            SearchFirstOrDefaultHandler = (_, __, ___) => expected
        };
        var endpoint = new ClienteEndpoint(db);

        var result = await endpoint.GetByEmail("x@y.com");

        var ok = Assert.IsType<OkObjectResult>(result);
        var cliente = Assert.IsType<Cliente>(ok.Value);
        Assert.Equal(expected.Email, cliente.Email);
    }

    [Fact]
    public async Task GetById_WhenNotFound_ReturnsNotFound()
    {
        var db = new FakeDbConnection
        {
            SearchFirstOrDefaultHandler = (_, __, ___) => null
        };
        var endpoint = new ClienteEndpoint(db);

        var result = await endpoint.GetById(123);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Cliente não encontrado", notFound.Value);
    }

    [Fact]
    public async Task GetById_WhenFound_ReturnsOkWithCliente()
    {
        var expected = new Cliente { IdCliente =123, Cpf = "000", Nome = "Id", Email = "id@test.com" };

        var db = new FakeDbConnection
        {
            SearchFirstOrDefaultHandler = (_, __, ___) => expected
        };
        var endpoint = new ClienteEndpoint(db);

        var result = await endpoint.GetById(123);

        var ok = Assert.IsType<OkObjectResult>(result);
        var cliente = Assert.IsType<Cliente>(ok.Value);
        Assert.Equal(123, cliente.IdCliente);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithClientes()
    {
        var expected = new[]
        {
            new Cliente { IdCliente =1, Cpf = "1", Nome = "A", Email = "a@a.com" },
            new Cliente { IdCliente =2, Cpf = "2", Nome = "B", Email = "b@b.com" },
        };

        var db = new FakeDbConnection
        {
            ListAllHandler = (_, __) => expected.Cast<object>()
        };
        var endpoint = new ClienteEndpoint(db);

        var result = await endpoint.GetAll();

        var ok = Assert.IsType<OkObjectResult>(result);
        var clientes = Assert.IsAssignableFrom<IEnumerable<Cliente>>(ok.Value);
        Assert.Equal(2, clientes.Count());
    }

    [Fact]
    public async Task InsertNew_WhenNullData_ThrowsArgumentException()
    {
        // Make the use case validation fail; it should return null and the endpoint should map that to BadRequest.
        var db = new FakeDbConnection();
        var endpoint = new ClienteEndpoint(db);

        var dto = new ClienteDto();
        await Assert.ThrowsAsync<ArgumentException>(async () => await endpoint.InsertNew(dto));
    }
}
