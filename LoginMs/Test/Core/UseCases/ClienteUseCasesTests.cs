using Core.Dtos;
using Core.Entities;
using Core.Interfaces.Gateways;
using Core.UseCases;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CoreNamespace = global::Core;

namespace Test.Core.UseCases;

public class ClienteUseCasesTests
{
    private const string Secret = "0123456789ABCDEF0123456789ABCDEF"; //32 chars

    [Fact]
    public async Task GetByEmail_WhenEmpty_ThrowsArgumentException()
    {
        var gw = new Mock<IClienteGateway>(MockBehavior.Strict);
        await Assert.ThrowsAsync<ArgumentException>(() => ClienteUseCases.GetByEmail(gw.Object, ""));
    }

    [Fact]
    public async Task GenerateClienteTokenByEmailCliente_WhenNotFound_Throws()
    {
        var gw = new Mock<IClienteGateway>();
        gw.Setup(x => x.GetByEmail("a@a.com")).ReturnsAsync((Cliente?)null);

        await Assert.ThrowsAsync<ArgumentException>(() => ClienteUseCases.GenerateClienteTokenByEmailCliente(gw.Object, Secret, "a@a.com"));
    }

    [Fact]
    public async Task GenerateClienteTokenByCpfCliente_ReturnsJwtWithRoleClienteIdentificado()
    {
        var gw = new Mock<IClienteGateway>();
        gw.Setup(x => x.GetByCpf("123")).ReturnsAsync(new Cliente { IdCliente = 1, Nome = "N", Email = "e@e.com", Cpf = "123" });

        var token = await ClienteUseCases.GenerateClienteTokenByCpfCliente(gw.Object, Secret, "123");

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        Assert.Contains(jwt.Claims, c => (c.Type == ClaimTypes.Role || c.Type == "role") && c.Value == CoreNamespace.Constants.UsuarioRoles.ClienteIdentificado);
    }

    [Fact]
    public async Task GenerateClienteAnonimoToken_ReturnsJwtWithRoleClienteAnonimo()
    {
        var gw = new Mock<IClienteGateway>();
        gw.Setup(x => x.InsertAnonymous()).ReturnsAsync(new Cliente { IdCliente = 99 });

        var token = await ClienteUseCases.GenerateClienteAnonimoToken(gw.Object, Secret);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        Assert.Contains(jwt.Claims, c => (c.Type == ClaimTypes.Role || c.Type == "role") &&
            c.Value == CoreNamespace.Constants.UsuarioRoles.ClienteAnonimo);
    }

    [Fact]
    public async Task InsertNewCliente_WhenDuplicateEmail_Throws()
    {
        var gw = new Mock<IClienteGateway>();
        gw.Setup(x => x.GetByEmail("e@e.com")).ReturnsAsync(new Cliente("N", "e@e.com", "123"));

        var dto = new ClienteDto { Nome = "N", Email = "e@e.com", Cpf = "123" };
        await Assert.ThrowsAsync<ArgumentException>(() => ClienteUseCases.InsertNewCliente(gw.Object, dto));
    }

    [Fact]
    public async Task InsertNewCliente_WhenValid_Inserts()
    {
        var gw = new Mock<IClienteGateway>();
        gw.Setup(x => x.GetByEmail("e@e.com")).ReturnsAsync((Cliente?)null);
        gw.Setup(x => x.Insert(It.IsAny<ClienteDto>())).ReturnsAsync(new Cliente("N", "e@e.com", "123") { IdCliente = 1 });

        var dto = new ClienteDto { Nome = "N", Email = "e@e.com", Cpf = "123" };
        var c = await ClienteUseCases.InsertNewCliente(gw.Object, dto);

        Assert.NotNull(c);
        gw.Verify(x => x.Insert(It.Is<ClienteDto>(d => d.Email == "e@e.com")), Times.Once);
    }
}
