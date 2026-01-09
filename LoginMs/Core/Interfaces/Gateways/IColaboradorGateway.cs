using Core.Dtos;
using Core.Entities;

namespace Core.Interfaces.Gateways
{
    public interface IColaboradorGateway
    {
        public Task<IEnumerable<Colaborador>> GetAll();
        public Task<Colaborador> Insert(ColaboradorDto colaboradorDto);
        public Task<Colaborador?> GetByEmail(string email);
        public Task<Colaborador?> GetByEmailAndSenha(string email, string senha);
        public Task<Colaborador?> GetById(int idColaborador);
        public Task Delete(Colaborador colaborador);
        public Task DeleteAll();
    }
}
