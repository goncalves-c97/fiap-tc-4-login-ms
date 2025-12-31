using Core.Dtos;
using Core.Entities;
using Core.Enums;
using Core.Gateways;
using Core.Helpers;
using Core.Interfaces.Gateways;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Core.UseCases
{
    public static class ColaboradorUseCases
    {
        public static async Task<string> GenerateColaboradorToken(IColaboradorGateway colaboradorGateway, string secret, ColaboradorDto colaboradorDto)
        {
            Colaborador? colaborador = await GetByEmailAndSenha(colaboradorGateway, colaboradorDto.Email, colaboradorDto.Senha) 
                ?? throw new ArgumentException("Colaborador não encontrado com o email e senha informados.");

            JwtSecurityTokenHandler tokenHandler = new();

            byte[] key = Encoding.ASCII.GetBytes(secret);

            var claims = new ClaimsIdentity([
                new Claim(ClaimTypes.NameIdentifier, colaborador.IdColaborador.ToString()),
                new Claim(ClaimTypes.Name, colaborador.Nome),
                new Claim(ClaimTypes.Email, colaborador.Email),
                new Claim(ClaimTypes.Role, colaborador.IdFuncao.ToString())
            ]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(12),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Task.FromResult(tokenHandler.WriteToken(token)).Result;
        }

        public static async Task<IEnumerable<Colaborador>> GetAllColaboradores(IColaboradorGateway colaboradorGateway)
        {
            return await colaboradorGateway.GetAll();
        }

        /// <summary>
        /// Retorna um colaborador baseado em seu usuário e senha.
        /// </summary>
        /// <param name="colaboradorGateway">Gateway de colaborador</param>
        /// <param name="email">Email do colaborador</param>
        /// <param name="senha">Senha em plain-text do colaborador (vai ser computado em hash)</param>
        /// <returns></returns>
        public static async Task<Colaborador?> GetByEmailAndSenha(IColaboradorGateway colaboradorGateway, string email, string senha)
        {
            senha = HashHelper.ComputeSha256Hash(senha);
            return await colaboradorGateway.GetByEmailAndSenha(email, senha);
        }

        public static async Task<Colaborador?> InsertNewColaborador(IColaboradorGateway colaboradorGateway, FuncaoColaboradorEnum funcaoColaboradorEnum, CadastroColaboradorDto cadastroColaboradorDto)
        {
            Colaborador colaborador = new((int)funcaoColaboradorEnum, cadastroColaboradorDto.Nome, cadastroColaboradorDto.Email, cadastroColaboradorDto.Senha);

            colaborador.ValidateValueObjects();

            if(!colaborador.IsValid)
                throw new ArgumentException("Colaborador inválido: " + colaborador.Errors.Summary);

            if (await colaboradorGateway.GetByEmail(cadastroColaboradorDto.Email) != null)
                throw new ArgumentException("Email já cadastrado no sistema!");

            ColaboradorDto colaboradorDto = new ColaboradorDto
            {
                IdFuncao = (int)funcaoColaboradorEnum,
                Email = cadastroColaboradorDto.Email,
                Nome = cadastroColaboradorDto.Nome,
                Senha = HashHelper.ComputeSha256Hash(cadastroColaboradorDto.Senha) 
            };

            return await colaboradorGateway.Insert(colaboradorDto);
        }
    
        public static async Task<Colaborador?> DeleteColaborador(IColaboradorGateway colaboradorGateway, int idColaborador)
        {
            Colaborador? colaborador = await colaboradorGateway.GetById(idColaborador);

            if (colaborador == null)
                return null;

            await colaboradorGateway.Delete(colaborador);

            return colaborador;
        }

        public static async Task DeleteAll(IColaboradorGateway colaboradorGateway)
        {
            await colaboradorGateway.DeleteAll();
        }
    }
}
