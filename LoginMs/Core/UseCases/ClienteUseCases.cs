using Core.Constants;
using Core.Dtos;
using Core.Entities;
using Core.Enums;
using Core.Gateways;
using Core.Helpers;
using Core.Interfaces.Gateways;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;

namespace Core.UseCases
{
    public static class ClienteUseCases
    {
        private static async Task<string> GenerateClienteToken(string secret, Cliente cliente)
        {
            JwtSecurityTokenHandler tokenHandler = new();

            byte[] key = Encoding.ASCII.GetBytes(secret);

            var claims = new ClaimsIdentity([
                new Claim(ClaimTypes.NameIdentifier, cliente.IdCliente.ToString()),
                new Claim(ClaimTypes.Name, cliente.Nome),
                new Claim(ClaimTypes.Email, cliente.Email),
                new Claim(ClaimTypes.Role, UsuarioRoles.ClienteIdentificado)
            ]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(12),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return await Task.FromResult(tokenHandler.WriteToken(token));
        }

        public static async Task<string> GenerateClienteTokenByEmailCliente(IClienteGateway clienteGateway, string secret, string email)
        {
            Cliente? cliente = await GetByEmail(clienteGateway, email) 
                ?? throw new ArgumentException("Cliente não encontrado com o email informado.", nameof(email));

            return await GenerateClienteToken(secret, cliente);
        }

        public static async Task<string> GenerateClienteTokenByCpfCliente(IClienteGateway clienteGateway, string secret, string cpf)
        {
            Cliente? cliente = await GetByCpf(clienteGateway, cpf) 
                ?? throw new ArgumentException("Cliente não encontrado com o CPF informado.", nameof(cpf));

            return await GenerateClienteToken(secret, cliente);
        }

        public static async Task<string> GenerateClienteAnonimoToken(IClienteGateway clienteGateway, string secret)
        {
            Cliente cliente = await GetNewAnonymous(clienteGateway) 
                ?? throw new ArgumentException("Não foi possível criar um cliente anônimo.");

            JwtSecurityTokenHandler tokenHandler = new();

            byte[] key = Encoding.ASCII.GetBytes(secret);

            var claims = new ClaimsIdentity([
                new Claim(ClaimTypes.NameIdentifier, cliente.IdCliente.ToString()),
                new Claim(ClaimTypes.Role, UsuarioRoles.ClienteAnonimo)
            ]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return await Task.FromResult(tokenHandler.WriteToken(token));
        }

        public static async Task<IEnumerable<Cliente>> GetAllClientes(IClienteGateway clienteGateway)
        {
            return await clienteGateway.GetAll();
        }

        public static async Task<Cliente?> GetByCpf(IClienteGateway clienteGateway, string cpf)
        {
            if (string.IsNullOrEmpty(cpf))
                throw new ArgumentException("CPF não pode ser nulo ou vazio.", nameof(cpf));

            return await clienteGateway.GetByCpf(cpf);
        }

        public static async Task<Cliente?> GetByEmail(IClienteGateway clienteGateway, string email)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentException("Email não pode ser nulo ou vazio.", nameof(email));

            return await clienteGateway.GetByEmail(email);
        }

        public static async Task<Cliente?> GetNewAnonymous(IClienteGateway clienteGateway)
        {
            return await clienteGateway.InsertAnonymous();
        }

        public static async Task<Cliente?> InsertNewCliente(IClienteGateway clienteGateway, ClienteDto clienteDto)
        {
            Cliente cliente = new(clienteDto.Nome, clienteDto.Email, clienteDto.Cpf);

            cliente.ValidateValueObjects();

            if (!cliente.IsValid)
                throw new ArgumentException("Cliente inválido: " + cliente.Errors.Summary);

            if (await clienteGateway.GetByEmail(clienteDto.Email) != null)
                throw new ArgumentException("Email já cadastrado no sistema!");

            return await clienteGateway.Insert(clienteDto);
        }

        public static async Task DeleteAll(IClienteGateway clienteGateway)
        {
            await clienteGateway.DeleteAll();
        }
    }
}