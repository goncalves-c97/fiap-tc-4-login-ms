using Core.Entities;

namespace Core.Interfaces
{
    public interface IDataSource
    {
        public Task<IEnumerable<Colaborador>> GetAllColaboradores();
    }
}
