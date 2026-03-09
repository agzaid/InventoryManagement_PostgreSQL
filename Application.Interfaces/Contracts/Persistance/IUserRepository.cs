using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Interfaces.Contracts.Persistance
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByEmailAsync(string email);
    }
}
