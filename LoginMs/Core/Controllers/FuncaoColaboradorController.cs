using Core.Dtos;
using Core.Gateways;
using Core.Interfaces;
using Core.UseCases;

namespace Core.Controllers
{
    public class FuncaoColaboradorController
    {
        public static async Task CheckBaseValuesInserted(IDbConnection dbConnection)
        {
            FuncaoColaboradorGateway gateway = new(dbConnection);
            await FuncaoColaboradorUseCases.CheckBaseValuesInserted(gateway);
        }
    }
}
