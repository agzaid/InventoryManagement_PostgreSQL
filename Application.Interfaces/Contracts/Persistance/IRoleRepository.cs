using Domain.Entities;

namespace Application.Interfaces.Contracts.Persistance
{
    public interface IRoleRepository : IGenericRepository<ApplicationRole>
    {
        Task<ApplicationRole?> GetByNameAsync(string name);
        Task<IEnumerable<ApplicationRole>> GetByUserIdAsync(string userId);
    }
}
