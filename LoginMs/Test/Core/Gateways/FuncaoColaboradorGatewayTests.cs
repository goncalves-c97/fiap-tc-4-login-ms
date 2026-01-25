using Core.Entities;
using Core.Gateways;
using Test.Helpers.Fakes;

namespace Test.Core.Gateways
{
    public class FuncaoColaboradorGatewayTests
    {
        [Fact]
        public async Task CheckBasefuncaoColaboradorInserted_WhenAlreadyHasRows_DoesNothing()
        {
            var fake = new FakeDbConnection();
            fake.ListAllHandler = (table, cols) => new object[] { new FuncaoColaborador { IdFuncao = 1, Funcao = "Administrador(a)" } };

            var gw = new FuncaoColaboradorGateway(fake);

            await gw.CheckBasefuncaoColaboradorInserted();

            Assert.Empty(fake.Inserts);
        }

        [Fact]
        public async Task CheckBasefuncaoColaboradorInserted_WhenEmpty_InsertsBaseFuncoes()
        {
            var fake = new FakeDbConnection();
            fake.ListAllHandler = (table, cols) => Array.Empty<object>();

            var gw = new FuncaoColaboradorGateway(fake);

            await gw.CheckBasefuncaoColaboradorInserted();

            // Due to current implementation using AddRange(funcoesColaborador) on itself,
            // it duplicates the entries (3 ->6). Tests document current behavior.
            Assert.Equal(6, fake.Inserts.Count);
            Assert.All(fake.Inserts, i => Assert.Equal("funcao_colaborador", i.Table));

            // should include all base roles
            var funcoes = fake.Inserts.Select(i => i.Values["funcao"].ToString()).ToList();
            Assert.Contains("Administrador(a)", funcoes);
            Assert.Contains("Cozinheiro(a)", funcoes);
            Assert.Contains("Atendente", funcoes);
        }
    }
}
