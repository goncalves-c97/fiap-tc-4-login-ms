using Core.Dtos;
using Core.Entities;

namespace Core.Interfaces.Gateways
{
    public interface IClienteGateway
    {
        public Task<IEnumerable<Cliente>> GetAll();
        public Task<Cliente> Insert(ClienteDto cliente);
        public Task<Cliente> InsertAnonymous();
        public Task<Cliente?> GetByCpf(string cpf);
        public Task<Cliente?> GetByEmail(string email);
        public Task<Cliente?> GetById(int idCliente);
        public Task DeleteAll();
    }
}
