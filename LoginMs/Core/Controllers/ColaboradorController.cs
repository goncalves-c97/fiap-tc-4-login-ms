using Core.Dtos;
using Core.Entities;
using Core.Enums;
using Core.Gateways;
using Core.Interfaces;
using Core.UseCases;

namespace Core.Controllers
{
    public static class ColaboradorController
    {
        public static async Task<string> GenerateColaboradorToken(IDbConnection dbConnection, string secret, ColaboradorDto colaboradorDto)
        {
            ColaboradorGateway gateway = new(dbConnection);
            string token = await ColaboradorUseCases.GenerateColaboradorToken(gateway, secret, colaboradorDto);
            return token;
        }

        public static async Task<IEnumerable<Colaborador>> GetAll(IDbConnection dbConnection)
        {
            ColaboradorGateway gateway = new(dbConnection);
            IEnumerable<Colaborador> colaboradores = await ColaboradorUseCases.GetAllColaboradores(gateway);
            return colaboradores;
        }

        public static async Task<Colaborador?> GetByEmailAndSenha(IDbConnection dbConnection, string email, string senha)
        {
            ColaboradorGateway gateway = new(dbConnection);
            Colaborador? colaborador = await ColaboradorUseCases.GetByEmailAndSenha(gateway, email, senha);
            return colaborador;
        }

        public static async Task<Colaborador?> InsertNewColaborador(IDbConnection dbConnection, FuncaoColaboradorEnum funcaoColaboradorEnum, CadastroColaboradorDto cadastroColaboradorDto)
        {
            ColaboradorGateway gateway = new(dbConnection);
            Colaborador? colaborador = await ColaboradorUseCases.InsertNewColaborador(gateway, funcaoColaboradorEnum, cadastroColaboradorDto);
            return colaborador;
        }

        public static async Task<Colaborador?> DeleteColaborador(IDbConnection dbConnection, int idColaborador)
        {
            ColaboradorGateway gateway = new(dbConnection);
            Colaborador? colaborador = await ColaboradorUseCases.DeleteColaborador(gateway, idColaborador);
            return colaborador;
        }

        public static async Task DeleteAll(IDbConnection dbConnection)
        {
            ColaboradorGateway gateway = new(dbConnection);
            await ColaboradorUseCases.DeleteAll(gateway);
        }
    }
}
