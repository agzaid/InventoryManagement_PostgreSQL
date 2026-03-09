using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Interfaces.Contracts.Persistance
{
    public interface IRoleRepository : IGenericRepository<Role>
    {
        Task<Role?> GetByNameAsync(string name);
    }
}
