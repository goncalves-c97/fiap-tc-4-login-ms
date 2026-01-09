using Core.Dtos;
using Core.Entities;
using Core.Enums;
using Core.Interfaces.Gateways;
using Core.UseCases;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Test.Core.UseCases;

public class ColaboradorUseCasesTests
{
    private const string Secret = "0123456789ABCDEF0123456789ABCDEF";

    [Fact]
    public async Task GetByEmailAndSenha_HashesPasswordBeforeCallingGateway()
    {
        var gw = new Mock<IColaboradorGateway>();
        string? capturedSenha = null;
        gw.Setup(x => x.GetByEmailAndSenha("e", It.IsAny<string>()))
        .Callback<string, string>((_, s) => capturedSenha = s)
        .ReturnsAsync((Colaborador?)null);

        await ColaboradorUseCases.GetByEmailAndSenha(gw.Object, "e", "plain");

        Assert.NotEqual("plain", capturedSenha);
        Assert.Matches("^[0-9a-f]{64}$", capturedSenha!);
    }

    [Fact]
    public async Task GenerateColaboradorToken_WhenNotFound_Throws()
    {
        var gw = new Mock<IColaboradorGateway>();
        gw.Setup(x => x.GetByEmailAndSenha(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((Colaborador?)null);

        await Assert.ThrowsAsync<ArgumentException>(() => ColaboradorUseCases.GenerateColaboradorToken(gw.Object, Secret, new ColaboradorDto { Email = "e", Senha = "s" }));
    }

    [Fact]
    public async Task GenerateColaboradorToken_WhenFound_ReturnsJwtWithClaims()
    {
        var gw = new Mock<IColaboradorGateway>();
        gw.Setup(x => x.GetByEmailAndSenha(It.IsAny<string>(), It.IsAny<string>()))
        .ReturnsAsync(new Colaborador(1, "Nome", "e@e.com", "hash") { IdColaborador = 10 });

        var token = await ColaboradorUseCases.GenerateColaboradorToken(gw.Object, Secret, new ColaboradorDto { Email = "e@e.com", Senha = "plain" });

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        Assert.Contains(jwt.Claims, c =>
            (c.Type == ClaimTypes.NameIdentifier || c.Type == "nameid") &&
            c.Value == "10");

        Assert.Contains(jwt.Claims, c =>
            (c.Type == ClaimTypes.Email || c.Type == JwtRegisteredClaimNames.Email) &&
            c.Value == "e@e.com");
    }

    [Fact]
    public async Task InsertNewColaborador_WhenDuplicateEmail_Throws()
    {
        var gw = new Mock<IColaboradorGateway>();
        gw.Setup(x => x.GetByEmail("e@e.com")).ReturnsAsync(new Colaborador(1, "N", "e@e.com", "s"));

        await Assert.ThrowsAsync<ArgumentException>(() => ColaboradorUseCases.InsertNewColaborador(gw.Object, FuncaoColaboradorEnum.Administrador,
        new CadastroColaboradorDto { Nome = "N", Email = "e@e.com", Senha = "s" }));
    }

    [Fact]
    public async Task DeleteColaborador_WhenNotFound_ReturnsNull()
    {
        var gw = new Mock<IColaboradorGateway>();
        gw.Setup(x => x.GetById(1)).ReturnsAsync((Colaborador?)null);

        var deleted = await ColaboradorUseCases.DeleteColaborador(gw.Object, 1);
        Assert.Null(deleted);
    }
}
