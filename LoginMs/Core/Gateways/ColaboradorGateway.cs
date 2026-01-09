using Core.Dtos;
using Core.Entities;
using Core.Interfaces;
using Core.Interfaces.Gateways;

namespace Core.Gateways
{
    public class ColaboradorGateway(IDbConnection dbConnection) : IColaboradorGateway
    {
        private readonly IDbConnection _dbConnection = dbConnection;
        private const string _tableName = nameof(Colaborador);

        public async Task Delete(Colaborador colaborador)
        {
            await _dbConnection.DeleteAsync(_tableName, "id_colaborador = @Id", new { Id = colaborador.IdColaborador });
        }

        public async Task<IEnumerable<Colaborador>> GetAll()
        {
            return await _dbConnection.ListAllAsync<Colaborador>(nameof(Colaborador));
        }

        public async Task<Colaborador?> GetByEmail(string email)
        {
            return await _dbConnection.SearchFirstOrDefaultByParametersAsync<Colaborador>(
                _tableName,
                "email = @Email",
                new { Email = email }
            );
        }

        public async Task<Colaborador?> GetByEmailAndSenha(string email, string senha)
        {
            return await _dbConnection.SearchFirstOrDefaultByParametersAsync<Colaborador>(
                _tableName,
                "email = @Email AND senha = @Senha",
                new { Email = email, Senha = senha }
            );
        }

        public async Task<Colaborador?> GetById(int idColaborador)
        {
            return await _dbConnection.SearchFirstOrDefaultByParametersAsync<Colaborador>(
                _tableName,
                "id_colaborador = @Id",
                new { Id = idColaborador }
            );
        }

        public async Task<Colaborador> Insert(ColaboradorDto colaboradorDto)
        {
            int registeredId = await _dbConnection.InsertAndReturnIdAsync(_tableName, new Dictionary<string, object>
            {
                { "id_funcao", colaboradorDto.IdFuncao },
                { "nome", colaboradorDto.Nome },
                { "email", colaboradorDto.Email },
                { "senha", colaboradorDto.Senha }
            }, "id_colaborador");

            return await GetById(registeredId);
        }

        public async Task DeleteAll()
        {
            await _dbConnection.DeleteAsync(_tableName, "1=1");
        }
    }
}
