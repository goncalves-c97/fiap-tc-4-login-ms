using Core.Controllers;
using Core.Dtos;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Endpoints
{
    [ApiController]
    [Route("Cliente")]
    public class ClienteEndpoint : ControllerBase
    {
        private readonly IDbConnection _dbConnection;

        public ClienteEndpoint(IDbConnection dbConnection) => _dbConnection = dbConnection;

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            IEnumerable<Cliente> clientes = await ClienteController.GetAll(_dbConnection);
            return Ok(clientes);
        }

        [HttpGet("GetByCpf/{cpf}")]
        public async Task<IActionResult> GetByCpf([FromRoute] string cpf)
        {
            Cliente? cliente = await ClienteController.GetByCpf(_dbConnection, cpf);

            if (cliente == null)
                return NotFound("Cliente não encontrado!");

            return Ok(cliente);
        }

        [HttpGet("GetByEmail/{email}")]
        public async Task<IActionResult> GetByEmail([FromRoute] string email)
        {
            Cliente? cliente = await ClienteController.GetByEmail(_dbConnection, email);

            if (cliente == null)
                return NotFound("Cliente não encontrado!");

            return Ok(cliente);
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            Cliente? cliente = await ClienteController.GetById(_dbConnection, id);

            if (cliente == null)
                return NotFound("Cliente não encontrado");

            return Ok(cliente);
        }

        [HttpPost("InsertNew")]
        public async Task<IActionResult> InsertNew([FromBody] ClienteDto clienteDto)
        {
            Cliente? insertedCliente = await ClienteController.InsertNewCliente(_dbConnection, clienteDto);

            if (insertedCliente == null)
                return BadRequest("Erro ao inserir novo cliente.");

            return Ok(insertedCliente);
        }
    }
}
