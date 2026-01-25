using Core.Dtos;
using Core.Entities;
using Core.Enums;
using Microsoft.AspNetCore.Mvc;
using Test.Helpers.Fakes;
using WebApi.Endpoints;

namespace Test.WebApi.Endpoints;

public class ColaboratorEndpointTests
{
    [Fact]
    public async Task GetAll_ReturnsOkWithList()
    {
        var db = new FakeDbConnection();
        db.ListAllHandler = (table, cols) => new List<object>
        {
            new Colaborador { IdColaborador =1, IdFuncao =1, Nome = "N", Email = "e@e.com", Senha = "s" }
        };

        var endpoint = new ColaboradorEndpoint(db);

        var result = await endpoint.GetAll();

        var ok = Assert.IsType<OkObjectResult>(result);
        var payload = Assert.IsAssignableFrom<IEnumerable<Colaborador>>(ok.Value);
        Assert.Single(payload);
    }

    [Fact]
    public async Task GetByEmailAndSenha_WhenNotFound_ReturnsNotFound()
    {
        var db = new FakeDbConnection();
        db.SearchFirstOrDefaultHandler = (_, _, _) => null;

        var endpoint = new ColaboradorEndpoint(db);

        var result = await endpoint.GetByEmailAndSenha("e@e.com", "pass");

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetByEmailAndSenha_WhenFound_ReturnsOk()
    {
        var db = new FakeDbConnection();
        db.SearchFirstOrDefaultHandler = (table, where, param) =>
        {
            if (table == "Colaborador" && where == "email = @Email AND senha = @Senha")
                return new Colaborador { IdColaborador =2, IdFuncao =1, Nome = "N2", Email = "e2@e.com", Senha = "hashed" };
            return null;
        };

        var endpoint = new ColaboradorEndpoint(db);

        var result = await endpoint.GetByEmailAndSenha("e2@e.com", "hashed");

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<Colaborador>(ok.Value);
    }

    [Fact]
    public async Task InsertNewColaborador_WhenInserted_ReturnsOk()
    {
        var db = new FakeDbConnection { NextInsertId =7 };
        db.SearchFirstOrDefaultHandler = (table, where, param) =>
        {
            // Duplicate email check
            if (table == "Colaborador" && where == "email = @Email")
                return null;

            // After insert, gateway queries by id
            if (table == "Colaborador" && where == "id_colaborador = @Id")
                return new Colaborador { IdColaborador =7, IdFuncao =1, Nome = "N", Email = "e@e.com", Senha = "hashed" };

            return null;
        };

        var endpoint = new ColaboradorEndpoint(db);

        var dto = new CadastroColaboradorDto { Nome = "N", Email = "e@e.com", Senha = "pass" };
        var result = await endpoint.InsertNewColaborador(FuncaoColaboradorEnum.Administrador, dto);

        var ok = Assert.IsType<OkObjectResult>(result);
        var c = Assert.IsType<Colaborador>(ok.Value);
        Assert.Equal(7, c.IdColaborador);
        Assert.Single(db.InsertAndReturnIds);
    }

    [Fact]
    public async Task DeleteColaborador_WhenNotFound_ReturnsNotFound()
    {
        var db = new FakeDbConnection();
        db.SearchFirstOrDefaultHandler = (table, where, param) => null;

        var endpoint = new ColaboradorEndpoint(db);

        var result = await endpoint.DeleteColaborador(123);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task DeleteColaborador_WhenFound_ReturnsOkAndDeletes()
    {
        var db = new FakeDbConnection();
        db.SearchFirstOrDefaultHandler = (table, where, param) =>
        {
            if (table == "Colaborador" && where == "id_colaborador = @Id")
                return new Colaborador { IdColaborador =3, IdFuncao =1, Nome = "N", Email = "e@e.com", Senha = "s" };
            return null;
        };

        var endpoint = new ColaboradorEndpoint(db);

        var result = await endpoint.DeleteColaborador(3);

        Assert.IsType<OkResult>(result);
        Assert.Single(db.Deletes);
        Assert.Equal("id_colaborador = @Id", db.Deletes[0].WhereClause);
    }
}
