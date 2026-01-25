using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Core.Helpers;
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

    [Fact]
    public async Task LoginAdministradorMock_WhenValid_ReturnsOkWithToken()
    {
        var db = new FakeDbConnection();
        db.SearchFirstOrDefaultHandler = (table, where, param) =>
        {
            // ColaboradorUseCases hashes provided senha before querying gateway
            // So DB comparison receives hashed senha.
            if (table == "Colaborador" && where.Contains("email = @Email") && where.Contains("senha = @Senha"))
                return new global::Core.Entities.Colaborador
                {
                    IdColaborador = 1,
                    IdFuncao = (int)global::Core.Enums.FuncaoColaboradorEnum.Administrador,
                    Nome = "ADMIN",
                    Email = "administrador@fastfoodchallenge.com.br",
                    Senha = HashHelper.ComputeSha256Hash(global::Core.Enums.FuncaoColaboradorEnum.Administrador.ToString())
                };

            return null;
        };

        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "API_AUTHENTICATION_KEY", "0123456789ABCDEF0123456789ABCDEF" }
        }).Build();

        var endpoint = new SetupEndpoint(db, config);

        var result = await endpoint.LoginAdministradorMock();

        var ok = Assert.IsType<OkObjectResult>(result);
        var tokenValue = ok.Value?.GetType().GetProperty("Token")?.GetValue(ok.Value) as string;
        Assert.False(string.IsNullOrWhiteSpace(tokenValue));

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(tokenValue);
        Assert.Contains(jwt.Claims, c => (c.Type == ClaimTypes.Email || c.Type == "email") && c.Value == "administrador@fastfoodchallenge.com.br");
    }

    [Fact]
    public async Task LoginCozinheiroMock_WhenValid_ReturnsOkWithToken()
    {
        var db = new FakeDbConnection();
        db.SearchFirstOrDefaultHandler = (table, where, param) =>
        {
            if (table == "Colaborador" && where.Contains("email = @Email") && where.Contains("senha = @Senha"))
                return new global::Core.Entities.Colaborador
                {
                    IdColaborador = 2,
                    IdFuncao = (int)global::Core.Enums.FuncaoColaboradorEnum.Cozinheiro,
                    Nome = "COZINHEIRO",
                    Email = "cozinheiro@fastfoodchallenge.com.br",
                    Senha = HashHelper.ComputeSha256Hash(global::Core.Enums.FuncaoColaboradorEnum.Cozinheiro.ToString())
                };

            return null;
        };

        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "API_AUTHENTICATION_KEY", "0123456789ABCDEF0123456789ABCDEF" }
        }).Build();

        var endpoint = new SetupEndpoint(db, config);

        var result = await endpoint.LoginCozinheiroMock();

        var ok = Assert.IsType<OkObjectResult>(result);
        var tokenValue = ok.Value?.GetType().GetProperty("Token")?.GetValue(ok.Value) as string;
        Assert.False(string.IsNullOrWhiteSpace(tokenValue));
    }

    [Fact]
    public async Task LoginAtendenteMock_WhenValid_ReturnsOkWithToken()
    {
        var db = new FakeDbConnection();
        db.SearchFirstOrDefaultHandler = (table, where, param) =>
        {
            if (table == "Colaborador" && where.Contains("email = @Email") && where.Contains("senha = @Senha"))
                return new global::Core.Entities.Colaborador
                {
                    IdColaborador = 3,
                    IdFuncao = (int)global::Core.Enums.FuncaoColaboradorEnum.Atendente,
                    Nome = "ATENDENTE",
                    Email = "atendente@fastfoodchallenge.com.br",
                    Senha = HashHelper.ComputeSha256Hash(global::Core.Enums.FuncaoColaboradorEnum.Atendente.ToString())
                };

            return null;
        };

        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "API_AUTHENTICATION_KEY", "0123456789ABCDEF0123456789ABCDEF" }
        }).Build();

        var endpoint = new SetupEndpoint(db, config);

        var result = await endpoint.LoginAtendenteMock();

        var ok = Assert.IsType<OkObjectResult>(result);
        var tokenValue = ok.Value?.GetType().GetProperty("Token")?.GetValue(ok.Value) as string;
        Assert.False(string.IsNullOrWhiteSpace(tokenValue));
    }
}
