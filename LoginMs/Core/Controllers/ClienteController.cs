using Core.Dtos;
using Core.Entities;
using Core.Gateways;
using Core.Interfaces;
using Core.UseCases;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Controllers
{
    public static class ClienteController
    {
        public static async Task<string> GenerateClienteTokenByEmail(IDbConnection dbConnection, string secret, string email)
        {
            ClienteGateway gateway = new(dbConnection);
            string token = await ClienteUseCases.GenerateClienteTokenByEmailCliente(gateway, secret, email);
            return token;
        }

        public static async Task<string> GenerateClienteTokenByCpf(IDbConnection dbConnection, string secret, string cpf)
        {
            ClienteGateway gateway = new(dbConnection);
            string token = await ClienteUseCases.GenerateClienteTokenByCpfCliente(gateway, secret, cpf);
            return token;
        }

        public static async Task<string> GenerateClienteAnonimoToken(IDbConnection dbConnection, string secret)
        {
            ClienteGateway gateway = new(dbConnection);
            string token = await ClienteUseCases.GenerateClienteAnonimoToken(gateway, secret);
            return token;
        }

        public static async Task<IEnumerable<Cliente>> GetAll(IDbConnection dbConnection)
        {
            ClienteGateway gateway = new(dbConnection);
            IEnumerable<Cliente> clientes = await ClienteUseCases.GetAllClientes(gateway);
            return clientes;
        }

        public static async Task<Cliente?> GetByCpf(IDbConnection dbConnection, string cpf)
        {
            ClienteGateway gateway = new(dbConnection);
            Cliente? cliente = await ClienteUseCases.GetByCpf(gateway, cpf);
            return cliente;
        }

        public static async Task<Cliente?> GetByEmail(IDbConnection dbConnection, string email)
        {
            ClienteGateway gateway = new(dbConnection);
            Cliente? cliente = await ClienteUseCases.GetByEmail(gateway, email);
            return cliente;
        }

        public static async Task<Cliente?> InsertNewCliente(IDbConnection dbConnection, ClienteDto clienteDto)
        {
            ClienteGateway gateway = new(dbConnection);
            Cliente? cliente = await ClienteUseCases.InsertNewCliente(gateway, clienteDto);
            return cliente;
        }

        public static async Task DeleteAll(IDbConnection dbConnection)
        {
            ClienteGateway gateway = new(dbConnection);
            await ClienteUseCases.DeleteAll(gateway);
        }
    }
}
