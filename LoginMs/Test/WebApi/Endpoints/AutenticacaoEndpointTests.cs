using System.IdentityModel.Tokens.Jwt;
using Core.Dtos;
using Core.Helpers;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Test.Helpers.Fakes;
using WebApi.Endpoints;

namespace Test.WebApi.Endpoints;

public class AutenticacaoEndpointTests
{
    private const string Secret = "0123456789ABCDEF0123456789ABCDEF";

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
            { "API_AUTHENTICATION_KEY", Secret }
        }).Build();

        var endpoint = new AutenticacaoEndpoint(db, config);
        endpoint.ModelState.AddModelError("x", "y");

        var result = await endpoint.LoginColaborador(new ColaboradorDto());
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task LoginColaborador_WhenValid_ReturnsOkWithToken()
    {
        var db = new FakeDbConnection();
        db.SearchFirstOrDefaultHandler = (table, where, param) =>
        {
            if (table == "Colaborador" && where == "email = @Email AND senha = @Senha")
            {
                return new global::Core.Entities.Colaborador
                {
                    IdColaborador = 1,
                    IdFuncao = 1,
                    Nome = "N",
                    Email = "e@e.com",
                    Senha = HashHelper.ComputeSha256Hash("pass")
                };
            }

            return null;
        };

        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "API_AUTHENTICATION_KEY", Secret }
        }).Build();

        var endpoint = new AutenticacaoEndpoint(db, config);

        var result = await endpoint.LoginColaborador(new ColaboradorDto { Email = "e@e.com", Senha = "pass" });

        var ok = Assert.IsType<OkObjectResult>(result);
        var tokenValue = ok.Value?.GetType().GetProperty("Token")?.GetValue(ok.Value) as string;
        Assert.False(string.IsNullOrWhiteSpace(tokenValue));
        _ = new JwtSecurityTokenHandler().ReadJwtToken(tokenValue);
    }

    [Fact]
    public async Task LoginClienteCpf_WhenModelStateInvalid_ReturnsBadRequest()
    {
        var db = new FakeDbConnection();
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "API_AUTHENTICATION_KEY", Secret }
        }).Build();

        var endpoint = new AutenticacaoEndpoint(db, config);
        endpoint.ModelState.AddModelError("x", "y");

        var result = await endpoint.LoginClienteCpf("123");
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task LoginClienteCpf_WhenValid_ReturnsOkWithToken()
    {
        var db = new FakeDbConnection();
        db.SearchFirstOrDefaultHandler = (table, where, param) =>
        {
            if (table == "Cliente" && where == "cpf = @Cpf")
                return new global::Core.Entities.Cliente { IdCliente = 10, Cpf = "123", Email = "e@e.com", Nome = "N" };

            return null;
        };

        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "API_AUTHENTICATION_KEY", Secret }
        }).Build();

        var endpoint = new AutenticacaoEndpoint(db, config);

        var result = await endpoint.LoginClienteCpf("123");

        var ok = Assert.IsType<OkObjectResult>(result);
        var tokenValue = ok.Value?.GetType().GetProperty("Token")?.GetValue(ok.Value) as string;
        Assert.False(string.IsNullOrWhiteSpace(tokenValue));
        _ = new JwtSecurityTokenHandler().ReadJwtToken(tokenValue);
    }

    [Fact]
    public async Task LoginClienteEmail_WhenModelStateInvalid_ReturnsBadRequest()
    {
        var db = new FakeDbConnection();
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "API_AUTHENTICATION_KEY", Secret }
        }).Build();

        var endpoint = new AutenticacaoEndpoint(db, config);
        endpoint.ModelState.AddModelError("x", "y");

        var result = await endpoint.LoginClienteEmail("e@e.com");
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task LoginClienteEmail_WhenValid_ReturnsOkWithToken()
    {
        var db = new FakeDbConnection();
        db.SearchFirstOrDefaultHandler = (table, where, param) =>
        {
            if (table == "Cliente" && where == "email = @Email")
                return new global::Core.Entities.Cliente { IdCliente = 11, Cpf = "123", Email = "e@e.com", Nome = "N" };

            return null;
        };

        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "API_AUTHENTICATION_KEY", Secret }
        }).Build();

        var endpoint = new AutenticacaoEndpoint(db, config);

        var result = await endpoint.LoginClienteEmail("e@e.com");

        var ok = Assert.IsType<OkObjectResult>(result);
        var tokenValue = ok.Value?.GetType().GetProperty("Token")?.GetValue(ok.Value) as string;
        Assert.False(string.IsNullOrWhiteSpace(tokenValue));
        _ = new JwtSecurityTokenHandler().ReadJwtToken(tokenValue);
    }

    [Fact]
    public async Task LoginClienteAnonimo_WhenModelStateInvalid_ReturnsBadRequest()
    {
        var db = new FakeDbConnection();
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "API_AUTHENTICATION_KEY", Secret }
        }).Build();

        var endpoint = new AutenticacaoEndpoint(db, config);
        endpoint.ModelState.AddModelError("x", "y");

        var result = await endpoint.LoginClienteAnonimo();
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task LoginClienteAnonimo_WhenValid_ReturnsOkWithToken()
    {
        var db = new FakeDbConnection
        {
            NextInsertId = 99,
            SearchFirstOrDefaultHandler = (table, where, param) =>
            {
                if (table == "Cliente" && where == "id_cliente = @Id")
                    return new global::Core.Entities.Cliente { IdCliente = 99 };

                return null;
            }
        };

        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "API_AUTHENTICATION_KEY", Secret }
        }).Build();

        var endpoint = new AutenticacaoEndpoint(db, config);

        var result = await endpoint.LoginClienteAnonimo();

        var ok = Assert.IsType<OkObjectResult>(result);
        var tokenValue = ok.Value?.GetType().GetProperty("Token")?.GetValue(ok.Value) as string;
        Assert.False(string.IsNullOrWhiteSpace(tokenValue));
        _ = new JwtSecurityTokenHandler().ReadJwtToken(tokenValue);

        Assert.Single(db.InsertAndReturnIds);
    }
}
