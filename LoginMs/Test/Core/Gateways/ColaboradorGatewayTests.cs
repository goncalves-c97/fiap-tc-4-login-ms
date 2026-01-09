using Core.Dtos;
using Core.Entities;
using Core.Gateways;
using Test.Helpers.Fakes;

namespace Test.Core.Gateways;

public class ColaboradorGatewayTests
{
    [Fact]
    public async Task GetByEmailAndSenha_CallsSearchWithExpectedWhere()
    {
        var fake = new FakeDbConnection();
        string? capturedWhere = null;
        fake.SearchFirstOrDefaultHandler = (table, where, param) => { capturedWhere = where; return null; };
        var gw = new ColaboradorGateway(fake);

        await gw.GetByEmailAndSenha("e", "s");

        Assert.Equal("email = @Email AND senha = @Senha", capturedWhere);
    }

    [Fact]
    public async Task Insert_InsertsAndReturnsById()
    {
        var fake = new FakeDbConnection { NextInsertId = 7 };
        fake.SearchFirstOrDefaultHandler = (table, where, param) => new Colaborador { IdColaborador = 7, Nome = "N", Email = "E", Senha = "S", IdFuncao = 1 };
        var gw = new ColaboradorGateway(fake);

        var dto = new ColaboradorDto { IdFuncao = 1, Nome = "N", Email = "E", Senha = "S" };
        var c = await gw.Insert(dto);

        Assert.Equal(7, c.IdColaborador);
        Assert.Single(fake.InsertAndReturnIds);
    }

    [Fact]
    public async Task Delete_DeletesById()
    {
        var fake = new FakeDbConnection();
        var gw = new ColaboradorGateway(fake);

        await gw.Delete(new Colaborador { IdColaborador = 3, IdFuncao = 1, Nome = "N", Email = "E", Senha = "S" });

        Assert.Single(fake.Deletes);
        Assert.Equal("id_colaborador = @Id", fake.Deletes[0].WhereClause);
    }
}
