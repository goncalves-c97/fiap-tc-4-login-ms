using Core.Controllers;
using Core.Dtos;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Endpoints
{
    [AllowAnonymous]
    [ApiController]
    [Route("Autenticacao")]
    public class AutenticacaoEndpoint : ControllerBase
    {
        private readonly IDbConnection _dbConnection;
        private readonly string _jwtSecret;

        public AutenticacaoEndpoint(IDbConnection dbConnection, IConfiguration configuration)
        {
            _dbConnection = dbConnection;

            string? jwtSecret = configuration["API_AUTHENTICATION_KEY"];

            if (string.IsNullOrEmpty(jwtSecret))
                throw new ArgumentException("A chave de autenticação da API não está configurada no appsettings.json.");

            _jwtSecret = jwtSecret;
        }

        [HttpPost, Route("LoginColaborador")]
        public async Task<IActionResult> LoginColaborador([FromBody] ColaboradorDto colaboradorDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string token = await ColaboradorController.GenerateColaboradorToken(_dbConnection, _jwtSecret, colaboradorDto);

            if (string.IsNullOrEmpty(token))
                return Unauthorized();
            else
                return Ok(new { Token = token });
        }

        [HttpPost, Route("LoginClienteCpf")]
        public async Task<IActionResult> LoginClienteCpf([FromQuery] string cpf)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string token = await ClienteController.GenerateClienteTokenByCpf(_dbConnection, _jwtSecret, cpf);

            if (string.IsNullOrEmpty(token))
                return Unauthorized();
            else
                return Ok(new { Token = token });
        }

        [HttpPost, Route("LoginClienteEmail")]
        public async Task<IActionResult> LoginClienteEmail([FromQuery] string email)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string token = await ClienteController.GenerateClienteTokenByEmail(_dbConnection, _jwtSecret, email);

            if (string.IsNullOrEmpty(token))
                return Unauthorized();
            else
                return Ok(new { Token = token });
        }

        [HttpPost, Route("LoginClienteAnonimo")]
        public async Task<IActionResult> LoginClienteAnonimo()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string token = await ClienteController.GenerateClienteAnonimoToken(_dbConnection, _jwtSecret);

            if (string.IsNullOrEmpty(token))
                return Unauthorized();
            else
                return Ok(new { Token = token });
        }
    }
}
