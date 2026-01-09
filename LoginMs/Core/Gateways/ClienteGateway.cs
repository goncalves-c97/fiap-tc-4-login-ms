using Core.Dtos;
using Core.Entities;
using Core.Interfaces;
using Core.Interfaces.Gateways;
using Microsoft.IdentityModel.Tokens;

namespace Core.Gateways
{
    public class ClienteGateway(IDbConnection dbConnection) : IClienteGateway
    {
        private readonly IDbConnection _dbConnection = dbConnection;
        private const string _tableName = nameof(Cliente);

        public async Task<IEnumerable<Cliente>> GetAll()
        {
            return await _dbConnection.ListAllAsync<Cliente>(nameof(Cliente));
        }

        public async Task<Cliente?> GetByCpf(string cpf)
        {
            return await _dbConnection.SearchFirstOrDefaultByParametersAsync<Cliente>(
                _tableName,
                "cpf = @Cpf",
                new { Cpf = cpf }
            );
        }

        public async Task<Cliente?> GetByEmail(string email)
        {
            return await _dbConnection.SearchFirstOrDefaultByParametersAsync<Cliente>(
                _tableName,
                "email = @Email",
                new { Email = email }
            );
        }

        public async Task<Cliente> Insert(ClienteDto cliente)
        {
            int registeredId = await _dbConnection.InsertAndReturnIdAsync(_tableName, new Dictionary<string, object>
            {
                { "cpf", cliente.Cpf },
                { "email", cliente.Email },
                { "nome", cliente.Nome },
                { "guid", Guid.NewGuid() }
            }, "id_cliente");

            return await GetById(registeredId);
        }

        public async Task<Cliente?> GetById(int idCliente)
        {
            return await _dbConnection.SearchFirstOrDefaultByParametersAsync<Cliente>(
                _tableName,
                "id_cliente = @Id",
                new { Id = idCliente }
            );
        }

        public async Task<Cliente> InsertAnonymous()
        {
            int registeredId = await _dbConnection.InsertAndReturnIdAsync(_tableName, new Dictionary<string, object>
            {
                { "guid", Guid.NewGuid() }
            }, "id_cliente");

            return await GetById(registeredId);
        }

        public async Task DeleteAll()
        {
            await _dbConnection.DeleteAsync(_tableName, "1=1");
        }
    }
}