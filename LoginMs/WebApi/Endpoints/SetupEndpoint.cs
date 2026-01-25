using Core.Controllers;
using Core.Dtos;
using Core.Enums;
using Core.Interfaces;
using Core.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Endpoints
{
    [AllowAnonymous]
    [ApiController]
    [Route("Setup")]
    public class SetupEndpoint : ControllerBase
    {
        private readonly IDbConnection _dbConnection;
        private readonly string _jwtSecret;

        public SetupEndpoint(IDbConnection dbConnection, IConfiguration configuration)
        {
            _dbConnection = dbConnection;

            string? jwtSecret = configuration["API_AUTHENTICATION_KEY"];

            if (string.IsNullOrEmpty(jwtSecret))
                throw new ArgumentException("A chave de autenticação da API não está configurada no appsettings.json.");

            _jwtSecret = jwtSecret;
        }

        [AllowAnonymous]
        [HttpDelete("ResetClientes")]
        public async Task<IActionResult> ResetClientes()
        {
            await ClienteController.DeleteAll(_dbConnection);
            return Ok();
        }

        [AllowAnonymous]
        [HttpDelete("ResetColaboradores")]
        public async Task<IActionResult> ResetColaboradores()
        {
            await ColaboradorController.DeleteAll(_dbConnection);
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("SetupColaboradoresMock")]
        public async Task<IActionResult> SetupColaboradoresMock()
        {
            foreach (FuncaoColaboradorEnum funcaoColaborador in Enum.GetValues<FuncaoColaboradorEnum>())
            {
                CadastroColaboradorDto colaborador = new()
                {
                    Nome = funcaoColaborador.ToString().ToUpper(),
                    Email = $"{funcaoColaborador}@fastfoodchallenge.com.br".ToLower(),
                    Senha = funcaoColaborador.ToString()
                };

                try
                {
                    await ColaboradorController.InsertNewColaborador(_dbConnection, funcaoColaborador, colaborador);
                }
                catch (ArgumentException ex)
                {
                    if (ex.Message.Contains("Email já cadastrado"))
                        continue; // Ignore if the email is already registered
                    else
                        throw;
                }
            }

            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("LoginAdministradorMock")]
        public async Task<IActionResult> LoginAdministradorMock()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            ColaboradorDto colaboradorDto = new()
            {
                Email = $"{FuncaoColaboradorEnum.Administrador}@fastfoodchallenge.com.br".ToLower(),
                Senha = FuncaoColaboradorEnum.Administrador.ToString()
            };

            string token = await ColaboradorController.GenerateColaboradorToken(_dbConnection, _jwtSecret, colaboradorDto);

            if (string.IsNullOrEmpty(token))
                return Unauthorized();
            else
                return Ok(new { Token = token });
        }

        [AllowAnonymous]
        [HttpPost("LoginCozinheiroMock")]
        public async Task<IActionResult> LoginCozinheiroMock()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            ColaboradorDto colaboradorDto = new()
            {
                Email = $"{FuncaoColaboradorEnum.Cozinheiro}@fastfoodchallenge.com.br",
                Senha = FuncaoColaboradorEnum.Cozinheiro.ToString()
            };

            string token = await ColaboradorController.GenerateColaboradorToken(_dbConnection, _jwtSecret, colaboradorDto);

            if (string.IsNullOrEmpty(token))
                return Unauthorized();
            else
                return Ok(new { Token = token });
        }

        [AllowAnonymous]
        [HttpPost("LoginAtendenteMock")]
        public async Task<IActionResult> LoginAtendenteMock()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            ColaboradorDto colaboradorDto = new()
            {
                Email = $"{FuncaoColaboradorEnum.Atendente}@fastfoodchallenge.com.br",
                Senha = FuncaoColaboradorEnum.Atendente.ToString()
            };

            string token = await ColaboradorController.GenerateColaboradorToken(_dbConnection, _jwtSecret, colaboradorDto);

            if (string.IsNullOrEmpty(token))
                return Unauthorized();
            else
                return Ok(new { Token = token });
        }

    }
}
