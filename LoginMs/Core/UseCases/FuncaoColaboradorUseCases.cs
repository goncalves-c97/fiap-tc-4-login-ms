using Core.Interfaces.Gateways;

namespace Core.UseCases
{
    public class FuncaoColaboradorUseCases
    {
        public static async Task CheckBaseValuesInserted(IFuncaoColaboradorGateway funcaoColaboradorGateway)
        {
            await funcaoColaboradorGateway.CheckBasefuncaoColaboradorInserted();
        }
    }
}
