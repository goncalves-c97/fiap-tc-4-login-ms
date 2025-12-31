using Core.Constants;
using Core.Controllers;
using Core.Dtos;
using Core.Entities;
using Core.Enums;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Endpoints
{
    [ApiController]
    [Route("Colaborador")]
    public class ColaboradorEndpoint : ControllerBase
    {
        private readonly IDbConnection _dbConnection;

        public ColaboradorEndpoint(IDbConnection dbConnection) => _dbConnection = dbConnection;

        [Authorize(Roles = UsuarioRoles.Administrador)]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll() 
        {
            IEnumerable<Colaborador> colaboradores = await ColaboradorController.GetAll(_dbConnection);
            return Ok(colaboradores);
        }

        [Authorize(Roles = UsuarioRoles.Administrador)]
        [HttpGet("GetByEmailAndSenha")]
        public async Task<IActionResult> GetByEmailAndSenha(string email, string senha)
        {
            Colaborador? colaborador = await ColaboradorController.GetByEmailAndSenha(_dbConnection, email, senha);
            
            if (colaborador == null)
                return NotFound("Colaborador não encontrado.");

            return Ok(colaborador);
        }

        [Authorize(Roles = UsuarioRoles.Administrador)]
        [HttpPost("InsertNewColaborador")]
        public async Task<IActionResult> InsertNewColaborador([FromQuery] FuncaoColaboradorEnum funcaoColaborador, [FromBody] CadastroColaboradorDto cadastroColaboradorDto)
        {
            Colaborador? colaborador = await ColaboradorController.InsertNewColaborador(_dbConnection, funcaoColaborador, cadastroColaboradorDto);

            if (colaborador == null)
                return BadRequest("Erro ao inserir colaborador.");

            return Ok(colaborador);
        }

        [Authorize(Roles = UsuarioRoles.Administrador)]
        [HttpDelete("DeleteColaborador")]
        public async Task<IActionResult> DeleteColaborador([FromQuery] int idColaborador)
        {
            Colaborador? colaboradorToDelete = await ColaboradorController.DeleteColaborador(_dbConnection, idColaborador);

            if (colaboradorToDelete == null)
                return NotFound("Não foi encontrado colaborador a partir do idColaborador informado!");

            return Ok();
        }
    }
}
